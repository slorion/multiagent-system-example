using DLC.Scientific.Core.Geocoding.Gps;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Factorization;
using System;
using System.Linq;

namespace DLC.Scientific.Core.Analysis.LinearAlgebra
{
	public static class PrimitivesFitting
	{
		public static bool ValidateDispersion(Matrix<double> data, int dispersionRatio)
		{
			if (data == null) throw new ArgumentNullException("data");

			if (data.RowCount < 3)
				return true;

			// Déterminer l'index du point central des données
			int centralIndex = Convert.ToInt32(Math.Floor((double) data.RowCount / 2));
			if (data.RowCount % 2 > 0)
				centralIndex++;
			centralIndex--; //Transcription matlab -> c# pour ramener le tout en base 0

			int firstHalfCount = centralIndex + 1;

			// Calculer la norme de la différence entre les points et le point central
			var dispersionNorm = Vector<double>.Build.Dense(data.RowCount);
			for (int i = 0; i < dispersionNorm.Count; i++)
				dispersionNorm[i] = (data.Row(i) - data.Row(centralIndex)).L2Norm();

			// Calculer l'inclinaison avant et après le point central
			double slopeBeforeCentral = (dispersionNorm.ToArray().Take(firstHalfCount).Max() - dispersionNorm[centralIndex]) / firstHalfCount;
			double slopeAfterCentral = (dispersionNorm.ToArray().Skip(firstHalfCount).Max() - dispersionNorm[centralIndex]) / (dispersionNorm.Count - firstHalfCount);

			double offDispersionBefore = 0;
			double offDispersionAfter = 0;
			double offDispersionRatio = 0;

			// Faire la somme des valeurs absolues des différences entre la différence et i * la valeur de référence
			for (int i = 0; i < dispersionNorm.Count; i++)
			{
				if (i <= centralIndex)
					offDispersionBefore += Math.Abs(dispersionNorm[i] - (centralIndex - i) * slopeBeforeCentral);
				else
					offDispersionAfter += Math.Abs(dispersionNorm[i] - (i - centralIndex) * slopeAfterCentral);
			}

			offDispersionRatio = offDispersionBefore / (dispersionNorm.ToArray().Take(firstHalfCount).Max() - dispersionNorm[centralIndex]);
			offDispersionRatio += offDispersionAfter / (dispersionNorm.ToArray().Skip(firstHalfCount).Max() - dispersionNorm[centralIndex]);

			if (offDispersionRatio > dispersionRatio)
				return false;

			return true;
		}

		public static LineData Ls2Dline(Matrix<double> data)
		{
			if (data == null) throw new ArgumentNullException("data");

			var dataTmp = data.Clone();
			LineData dataOut = new LineData(dataTmp.RowCount);

			// Check number of data points
			if (dataTmp.RowCount < 2)
				return dataOut;

			// Calculate centroid
			dataOut.Centroid[0] = 0;
			dataOut.Centroid[1] = 0;

			for (int i = 0; i < dataTmp.RowCount; i++)
			{
				dataOut.Centroid[0] += dataTmp[i, 0];
				dataOut.Centroid[1] += dataTmp[i, 1];
			}

			dataOut.Centroid[0] /= dataTmp.RowCount;
			dataOut.Centroid[1] /= dataTmp.RowCount;

			// Form matrix a of translated points
			for (int i = 0; i < dataTmp.RowCount; i++)
			{
				dataTmp[i, 0] -= dataOut.Centroid[0];
				dataTmp[i, 1] -= dataOut.Centroid[1];
			}

			var svd = dataTmp.Svd(computeVectors: true);

			// Find the largest singular value in S and extract from V the
			// corresponding right singular vector

			dataOut.LineParams[0] = svd.VT[0, 0];
			dataOut.LineParams[1] = svd.VT[0, 1];

			// Calculate residual distances
			for (int i = 0; i < dataTmp.RowCount; i++)
			{
				var vDiff = Vector<double>.Build.Dense(new[] { dataTmp[i, 0], dataTmp[i, 1] });

				dataOut.Residuals[i] = MatrixOperation.CrossProduct(vDiff, dataOut.LineParams).L2Norm();
				dataOut.ResidualsNorm += Math.Pow(dataOut.Residuals[i], 2);
			}

			dataOut.ResidualsNorm = Math.Sqrt(dataOut.ResidualsNorm);

			// Calculate intersection point
			var startPoint = Vector<double>.Build.Dense(new[] { data[0, 0], data[0, 1] });

			var endPoint = Vector<double>.Build.Dense(new[] { data[data.RowCount - 1, 0], data[data.RowCount - 1, 1] });

			var intersectionPoint = GetIntersectionPoint(startPoint, endPoint, dataOut.LineParams, dataOut.Centroid);
			if (intersectionPoint == null)
			{
				//TODO: JFF: À revoir... Ajouté parce qu'il arrivait que lfU soit non numérique et que si les intersections restaient à 0 alors d'autres erreurs arrivaient plus loin. Pour l'instant c'est la correction la plus simple mais lorsqu'on aura un peu plus de temps, se pencher d'avantage sur une correction/gestion plus viable de cette erreur. Erreur qui arrivait par exemple lorsque la matrix d'entré contenait 2 coordonnées et que ses dernières étaient exactement les mêmes. 
				dataOut.IntersectionPoint[0] = data[data.RowCount - 1, 0];
				dataOut.IntersectionPoint[1] = data[data.RowCount - 1, 1];
			}

			return dataOut;
		}

		public static Vector<double> GetCartesianPoint(Vector<double> currentCartesianPoint, LineData lineData, int nbStep, double stepLength)
		{
			if (lineData == null) throw new ArgumentNullException("lineData");

			var newPoint = Vector<double>.Build.Dense(new[] {
				nbStep * stepLength * lineData.LineParams[0] + currentCartesianPoint[0],
				nbStep * stepLength * lineData.LineParams[1] + currentCartesianPoint[1],
				0
			});

			return newPoint;
		}

		public static Line3DData Ls3Dline(Matrix<double> data)
		{
			if (data == null) throw new ArgumentNullException("data");

			return Ls3Dline(data, data.RowCount - 1);
		}

		public static Line3DData Ls3Dline(Matrix<double> data, int startPointIndex)
		{
			if (data == null) throw new ArgumentNullException("data");

			//-------------------------------------------------------------
			// DÉBUT Fit 3DLine
			//-------------------------------------------------------------

			var dataTmp = data.Clone();
			Line3DData dataOut = new Line3DData(dataTmp.RowCount);

			// Check number of data points
			if (dataTmp.RowCount < 2)
				return dataOut;

			// Calculate centroid
			dataOut.Centroid[0] = 0;
			dataOut.Centroid[1] = 0;
			dataOut.Centroid[2] = 0;

			for (int i = 0; i < dataTmp.RowCount; i++)
			{
				dataOut.Centroid[0] += dataTmp[i, 0];
				dataOut.Centroid[1] += dataTmp[i, 1];
				dataOut.Centroid[2] += dataTmp[i, 2];
			}

			dataOut.Centroid[0] /= dataTmp.RowCount;
			dataOut.Centroid[1] /= dataTmp.RowCount;
			dataOut.Centroid[2] /= dataTmp.RowCount;

			// Form matrix a of translated points
			for (int i = 0; i < dataTmp.RowCount; i++)
			{
				dataTmp[i, 0] -= dataOut.Centroid[0];
				dataTmp[i, 1] -= dataOut.Centroid[1];
				dataTmp[i, 2] -= dataOut.Centroid[2];
			}

			var svd = dataTmp.Svd(computeVectors: true);

			// Find the largest singular value in S and extract from V the
			// corresponding right singular vector
			dataOut.LineParams[0] = svd.VT[0, 0];
			dataOut.LineParams[1] = svd.VT[0, 1];
			dataOut.LineParams[2] = svd.VT[0, 2];

			// Calculate residual distances
			for (int i = 0; i < dataTmp.RowCount; i++)
			{
				var vDiff = Vector<double>.Build.Dense(new[] { dataTmp[i, 0], dataTmp[i, 1], dataTmp[i, 2] });
				dataOut.Residuals[i] = MatrixOperation.CrossProduct(vDiff, dataOut.LineParams).L2Norm();
			}

			dataOut.ResidualsNorm = dataOut.Residuals.L2Norm();

			//-------------------------------------------------------------
			// FIN Fit 3DLine
			//-------------------------------------------------------------

			//TODO: Michel Robert: Il faut refactoriser, il s'agit de 2 concepts, donc 2 fonctions

			//-------------------------------------------------------------
			// Début Extrapolation
			//-------------------------------------------------------------

			// Calculate intersection point
			var startPoint = Vector<double>.Build.Dense(new[] { data[0, 0], data[0, 1], data[0, 2] });
			var endPoint = Vector<double>.Build.Dense(new[] { data[startPointIndex, 0], data[startPointIndex, 1], data[startPointIndex, 2] });

			var intersectionPoint = GetIntersectionPoint(startPoint, endPoint, dataOut.LineParams, dataOut.Centroid);
			if (intersectionPoint == null)
			{
				//TODO: JFF: À revoir... Ajouté parce qu'il arrivait que lfU soit non numérique et que si 
				// les intersections restaient à 0 alors d'autres erreurs arrivaient plus loin. 
				// Pour l'instant c'est la correction la plus simple mais lorsqu'on aura un peu plus de temps, 
				// se pencher d'avantage sur une correction/gestion plus viable de cette erreur. 
				// Erreur qui arrivait par exemple lorsque la matrix d'entrée contenait 2 coordonnées et que 
				// ces dernières étaient exactement les mêmes. 
				dataOut.IntersectionPoint = endPoint;
			}
			else
				dataOut.IntersectionPoint = intersectionPoint;


			if (!GetLineDirection(startPoint, dataOut.IntersectionPoint, dataOut.LineParams))
				dataOut.LineParams = dataOut.LineParams * -1;

			return dataOut;
		}

		public static Vector<double> GetCartesianPoint(Vector<double> currentCartesianPoint, Line3DData line3DData, int nbStep, double stepLength)
		{
			return Vector<double>.Build.Dense(new[] {
				nbStep * stepLength * line3DData.LineParams[0] + currentCartesianPoint[0],
				nbStep * stepLength * line3DData.LineParams[1] + currentCartesianPoint[1],
				nbStep * stepLength * line3DData.LineParams[2] + currentCartesianPoint[2]
			});
		}

		public static PlaneData LsPlane(Matrix<double> data)
		{
			if (data == null) throw new ArgumentNullException("data");

			var dataTmp = data.Clone();
			PlaneData dataOut = new PlaneData(dataTmp.RowCount);

			// Check number of data points
			if (dataTmp.RowCount < 3)
				return dataOut;

			// Calculate centroid
			dataOut.Centroid[0] = 0;
			dataOut.Centroid[1] = 0;
			dataOut.Centroid[2] = 0;

			for (int i = 0; i < dataTmp.RowCount; i++)
			{
				dataOut.Centroid[0] += dataTmp[i, 0];
				dataOut.Centroid[1] += dataTmp[i, 1];
				dataOut.Centroid[2] += dataTmp[i, 2];
			}

			dataOut.Centroid[0] /= dataTmp.RowCount;
			dataOut.Centroid[1] /= dataTmp.RowCount;
			dataOut.Centroid[2] /= dataTmp.RowCount;

			// Form matrix a of translated points
			for (int i = 0; i < dataTmp.RowCount; i++)
			{
				dataTmp[i, 0] -= dataOut.Centroid[0];
				dataTmp[i, 1] -= dataOut.Centroid[1];
				dataTmp[i, 2] -= dataOut.Centroid[2];
			}

			var svd = dataTmp.Svd(computeVectors: true);

			// Direction cosines of the normal to the best-fit plane.	
			// Find the smallest singular value in S and extract from V the
			// corresponding right singular vector.
			dataOut.PlaneParams[0] = svd.VT[2, 0];
			dataOut.PlaneParams[1] = svd.VT[2, 1];
			dataOut.PlaneParams[2] = svd.VT[2, 2];

			return dataOut;
		}

		public static CircleData Ls2DCircle(Matrix<double> data)
		{
			if (data == null) throw new ArgumentNullException("data");

			var dataTmp = data.Clone();
			var dataOut = new CircleData();

			// Check number of data points
			if (dataTmp.RowCount < 2)
				return dataOut;

			// init A and b
			var A = Matrix<double>.Build.Dense(dataTmp.RowCount, 3);
			var b = Vector<double>.Build.Dense(dataTmp.RowCount);

			for (int i = 0; i < dataTmp.RowCount; i++)
			{
				A[i, 0] = -2 * dataTmp[i, 0];
				A[i, 1] = -2 * dataTmp[i, 1];
				A[i, 2] = 1.0f;

				b[i] = -(Math.Pow(dataTmp[i, 0], 2) + Math.Pow(dataTmp[i, 1], 2));
			}

			Vector<double> C = A.QR(QRMethod.Full).Solve(b);

			// set circle centroid
			dataOut.CircleCentroid[0] = C[0];
			dataOut.CircleCentroid[1] = C[1];

			// compute circle radius
			dataOut.CircleRadius = Math.Sqrt(Math.Pow(C[0], 2) + Math.Pow(C[1], 2) - C[2]);

			// Convert last to polar coordinates
			Vector<double> lastPolar = GpsHelper.ConvertCartesianToPolar(dataTmp[dataTmp.RowCount - 1, 0], dataTmp[dataTmp.RowCount - 1, 1], dataTmp[dataTmp.RowCount - 1, 2]);
			dataOut.Inclination = lastPolar[1];

			return dataOut;
		}

		public static Vector<double> GetCartesianPoint(Vector<double> currentCartesianPoint, CircleData circleData, int nbStep, double stepLength)
		{
			if (circleData == null) throw new ArgumentNullException("circleData");

			double initialTheta = GpsHelper.ConvertCartesianToPolar(currentCartesianPoint[0], currentCartesianPoint[1])[1];
			double theta = initialTheta + nbStep * stepLength;

			return Vector<double>.Build.Dense(new[] {
				circleData.CircleRadius * Math.Cos(theta) + circleData.CircleCentroid[0],
				circleData.CircleRadius * Math.Sin(theta) + circleData.CircleCentroid[1],
				0
			});
		}

		public static Circle3DData Ls3DCircle(Matrix<double> data)
		{
			if (data == null) throw new ArgumentNullException("data");

			return Ls3DCircle(data, data.RowCount - 1);
		}

		public static Circle3DData Ls3DCircle(Matrix<double> data, int startPointIndex)
		{
			if (data == null) throw new ArgumentNullException("data");

			var dataTmp = data.Clone();
			var dataOut = new Circle3DData();

			// Check number of data points
			if (dataTmp.RowCount < 2)
				return dataOut;

			PlaneData planeData = LsPlane(dataTmp);
			dataOut.PlaneCentroid = planeData.Centroid;

			// Form matrix A of translated points
			for (int i = 0; i < dataTmp.RowCount; i++)
			{
				dataTmp[i, 0] -= dataOut.PlaneCentroid[0];
				dataTmp[i, 1] -= dataOut.PlaneCentroid[1];
				dataTmp[i, 2] -= dataOut.PlaneCentroid[2];
			}

			// Transform the data to close to standard position via a rotation 
			// followed by a translation 
			dataOut.RotationMatrix = MatrixOperation.RotationZ_3D(planeData.PlaneParams);
			dataOut.RotationCentroid = dataOut.RotationMatrix * dataOut.PlaneCentroid;
			Matrix<double> rotatedData = (dataOut.RotationMatrix * dataTmp.Transpose()).Transpose();

			//Translate data to the rotated origin 
			for (int i = 0; i < dataTmp.RowCount; i++)
			{
				rotatedData[i, 0] -= dataOut.RotationCentroid[0];
				rotatedData[i, 1] -= dataOut.RotationCentroid[1];
				rotatedData[i, 2] -= dataOut.RotationCentroid[2];
			}

			CircleData circle2DData = Ls2DCircle(rotatedData);
			dataOut.CircleCentroid = circle2DData.CircleCentroid;
			dataOut.CircleRadius = circle2DData.CircleRadius;

			// THETA is a counterclockwise angular displacement in radians from the
			// positive x-axis, RHO is the distance from the origin to a point in the x-y plane 
			for (int i = 0; i < rotatedData.RowCount; i++)
			{
				rotatedData[i, 0] -= dataOut.CircleCentroid[0];
				rotatedData[i, 1] -= dataOut.CircleCentroid[1];
			}

			//Convert start point to polar coordinate
			Vector<double> startPolar = GpsHelper.ConvertCartesianToPolar(rotatedData[startPointIndex, 0], rotatedData[startPointIndex, 1], rotatedData[startPointIndex, 2]);

			dataOut.IsAscending = GetCircleDirection(rotatedData.Row(0), rotatedData.Row(startPointIndex), startPolar[0], dataOut.CircleRadius);
			dataOut.Theta = startPolar[0];
			dataOut.Z = startPolar[2];

			return dataOut;
		}

		public static Vector<double> GetCartesianPoint(Vector<double> currentCartesianPoint, Circle3DData circle3DData, int nbStep, double stepLength)
		{
			if (circle3DData == null) throw new ArgumentNullException("circle3DData");

			double circumference = 2 * Math.PI * circle3DData.CircleRadius;
			double theta = stepLength * 2 * Math.PI / circumference;

			if (circle3DData.IsAscending)
				circle3DData.Theta = circle3DData.Theta + theta;
			else
				circle3DData.Theta = circle3DData.Theta - theta;

			Vector<double> newPoint = GpsHelper.ConvertPolarToCartesian(circle3DData.CircleRadius, circle3DData.Theta, circle3DData.Z);

			// Map to the initial coordinates system
			var pointMatrix = Matrix<double>.Build.Dense(1, 3);
			pointMatrix[0, 0] = newPoint[0] + circle3DData.CircleCentroid[0] + circle3DData.RotationCentroid[0];
			pointMatrix[0, 1] = newPoint[1] + circle3DData.CircleCentroid[1] + circle3DData.RotationCentroid[1];
			pointMatrix[0, 2] = newPoint[2] + circle3DData.RotationCentroid[2];

			pointMatrix = (circle3DData.RotationMatrix.Transpose() * pointMatrix.Transpose()).Transpose();

			newPoint[0] = pointMatrix[0, 0] + circle3DData.PlaneCentroid[0];
			newPoint[1] = pointMatrix[0, 1] + circle3DData.PlaneCentroid[1];
			newPoint[2] = pointMatrix[0, 2] + circle3DData.PlaneCentroid[2];

			return newPoint;
		}

		public static PolynomialData LsPolynomial(Vector<double> data, Vector<double> index, int order)
		{
			if (data == null) throw new ArgumentNullException("data");
			if (index == null) throw new ArgumentNullException("index");

			if (order != 3 && order != 5)
				throw new NotImplementedException("Uniquement le 3e et le 5e ordre sont implantés");

			var dataOut = new PolynomialData(order);

			Matrix<double> V = MatrixOperation.VandermondeMatrix(index, order);

			Vector<double> p = V.QR(QRMethod.Full).Solve(data);
			Vector<double> r = data - (V * p);

			dataOut.PolynomialParams = Vector<double>.Build.DenseOfArray(p.ToArray());
			dataOut.VandermondeMatrix = V;
			dataOut.FreedomDegrees = data.Count - (order + 1);
			dataOut.ResidualNorm = r.L2Norm();
			dataOut.PolynomialOrder = order;

			return dataOut;
		}

		public static Vector<double> GetCartesianPoint(PolynomialData polyDataLatitude, PolynomialData polyDataLongitude, PolynomialData polyDataAltitude, int nbStep)
		{
			if (polyDataLatitude == null) throw new ArgumentNullException("polyDataLatitude");
			if (polyDataLongitude == null) throw new ArgumentNullException("polyDataLongitude");
			if (polyDataAltitude == null) throw new ArgumentNullException("polyDataAltitude");

			return Vector<double>.Build.Dense(new[] {
				GetPolynomialValue(nbStep, polyDataLatitude.PolynomialParams, polyDataLatitude.PolynomialOrder),
				GetPolynomialValue(nbStep, polyDataLongitude.PolynomialParams, polyDataLongitude.PolynomialOrder),
				GetPolynomialValue(nbStep, polyDataAltitude.PolynomialParams, polyDataAltitude.PolynomialOrder)
			});
		}

		public static Vector<double> GetIntersectionPoint(Vector<double> startPoint, Vector<double> referencePoint, Vector<double> lineParams, Vector<double> centroid)
		{
			if (startPoint == null) throw new ArgumentNullException("startPoint");
			if (referencePoint == null) throw new ArgumentNullException("referencePoint");
			if (lineParams == null) throw new ArgumentNullException("lineParams");
			if (centroid == null) throw new ArgumentNullException("centroid");

			int nbDimension = lineParams.Count;
			var maxPoint = Vector<double>.Build.Dense(nbDimension);
			var intersectionPoint = Vector<double>.Build.Dense(nbDimension);

			maxPoint[0] = Math.Abs(referencePoint[0] - startPoint[0]);
			maxPoint[1] = Math.Abs(referencePoint[1] - startPoint[1]);
			if (nbDimension > 2)
				maxPoint[2] = Math.Abs(referencePoint[2] - startPoint[2]);

			double normXYZ = maxPoint.L2Norm();
			double xLower = centroid[0] - normXYZ * lineParams[0];
			double yLower = centroid[1] - normXYZ * lineParams[1];
			double zLower = 0;
			double xUpper = centroid[0] + normXYZ * lineParams[0];
			double yUpper = centroid[1] + normXYZ * lineParams[1];
			double zUpper = 0;

			double lfLineMag = -1;
			double lfU = -1;

			if (nbDimension > 2)
			{
				zLower = centroid[2] - normXYZ * lineParams[2];
				zUpper = centroid[2] + normXYZ * lineParams[2];

				lfLineMag = Math.Sqrt(Math.Pow(xUpper - xLower, 2) + Math.Pow(yUpper - yLower, 2) + Math.Pow(zUpper - zLower, 2));
				lfU = (((referencePoint[0] - xLower) * (xUpper - xLower)) + ((referencePoint[1] - yLower) * (yUpper - yLower)) + ((referencePoint[2] - zLower) * (zUpper - zLower))) / (lfLineMag * lfLineMag);
			}
			else
			{
				lfLineMag = Math.Sqrt(Math.Pow(xUpper - xLower, 2) + Math.Pow(yUpper - yLower, 2));
				lfU = (((referencePoint[0] - xLower) * (xUpper - xLower)) + ((referencePoint[1] - yLower) * (yUpper - yLower))) / (lfLineMag * lfLineMag);
			}

			if (lfU >= 0 && lfU <= 1)
			{
				intersectionPoint[0] = xLower + lfU * (xUpper - xLower);
				intersectionPoint[1] = yLower + lfU * (yUpper - yLower);
				if (nbDimension > 2)
					intersectionPoint[2] = zLower + lfU * (zUpper - zLower);

				return intersectionPoint;
			}

			return null;
		}

		private static bool GetCircleDirection(Vector<double> first, Vector<double> last, double theta, double circleRadius)
		{
			double circumference = 2 * Math.PI * circleRadius;
			double thetaOffset = 2 * Math.PI / circumference;

			Vector<double> newPointMinus = GpsHelper.ConvertPolarToCartesian(circleRadius, theta - thetaOffset, last[2]);
			Vector<double> newPointPlus = GpsHelper.ConvertPolarToCartesian(circleRadius, theta + thetaOffset, last[2]);

			return (newPointMinus - first).Count <= (newPointPlus - first).Count;
		}

		private static bool GetLineDirection(Vector<double> first, Vector<double> last, Vector<double> lineParams)
		{
			if (first == null) throw new ArgumentNullException("first");
			if (last == null) throw new ArgumentNullException("last");
			if (lineParams == null) throw new ArgumentNullException("lineParams");

			var newPointPlus = Vector<double>.Build.Dense(new[] { last[0] + lineParams[0], last[1] + lineParams[1], last[2] + lineParams[2] });
			var newPointMinus = Vector<double>.Build.Dense(new[] { last[0] + (lineParams[0] * -1), last[1] + (lineParams[1] * -1), last[2] + (lineParams[2] * -1) });

			return (newPointMinus - first).L2Norm() <= (newPointPlus - first).L2Norm();
		}

		private static double GetPolynomialValue(double value, Vector<double> polyParams, int order)
		{
			if (polyParams == null) throw new ArgumentNullException("polyParams");

			if (order == 3)
			{
				return
					Math.Pow(value, 3) * polyParams[0]
					+ Math.Pow(value, 2) * polyParams[1]
					+ value * polyParams[2]
					+ polyParams[3];
			}
			else if (order == 5)
			{
				return
					Math.Pow(value, 5) * polyParams[0]
					+ Math.Pow(value, 4) * polyParams[1]
					+ Math.Pow(value, 3) * polyParams[2]
					+ Math.Pow(value, 2) * polyParams[3]
					+ value * polyParams[4]
					+ polyParams[5];
			}
			else
				return 0;
		}
	}
}