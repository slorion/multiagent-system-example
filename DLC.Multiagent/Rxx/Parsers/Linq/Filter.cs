﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Rxx.Parsers.Linq
{
  public static partial class Parser
  {
    /// <summary>
    /// Matches all results from the specified <paramref name="parser"/> for which the specified 
    /// <paramref name="predicate"/> returns <see langword="true"/>.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <param name="parser">The parser from which matches will be filtered by the specified <paramref name="predicate"/>.</param>
    /// <param name="predicate">A function that returns <see langword="true"/> to indicate when a match should be yielded and 
    /// <see langword="false"/> when a match should be ignored.</param>
    /// <returns>A parser that matches only those results from the specified <paramref name="parser"/> for which 
    /// the specified <paramref name="predicate"/> returns <see langword="true"/>.</returns>
    public static IParser<TSource, TResult> Where<TSource, TResult>(
      this IParser<TSource, TResult> parser,
      Func<TResult, bool> predicate)
    {
      Contract.Requires(parser != null);
      Contract.Requires(predicate != null);
      Contract.Ensures(Contract.Result<IParser<TSource, TResult>>() != null);

      return parser.Yield("Where", source => parser.Parse(source).Where(result => predicate(result.Value)));
    }

    /// <summary>
    /// Matches the left parser followed by the right parser, but only returns the right parser's matches.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TIgnore">The type of the elements that are generated by the left parser.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated by the right parser.</typeparam>
    /// <param name="ignoreParser">The parser from which to ignore matches.</param>
    /// <param name="parser">The parser from which to yield matches.</param>
    /// <returns>A parser that matches the left parser followed by the right parser, but only returns the 
    /// right parser's matches.</returns>
    public static IParser<TSource, TResult> IgnoreBefore<TSource, TIgnore, TResult>(
      this IParser<TSource, TIgnore> ignoreParser,
      IParser<TSource, TResult> parser)
    {
      Contract.Requires(ignoreParser != null);
      Contract.Requires(parser != null);
      Contract.Ensures(Contract.Result<IParser<TSource, TResult>>() != null);

      return ignoreParser.SelectMany(_ => parser, (_, trailing) => trailing);
    }

    /// <summary>
    /// Matches the left parser followed by the right parser, but only returns the left parser's matches.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TIgnore">The type of the elements that are generated by the right parser.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated by the left parser.</typeparam>
    /// <param name="parser">The parser from which to yield matches.</param>
    /// <param name="ignoreParser">The parser from which to ignore matches.</param>
    /// <returns>A parser that matches the left parser followed by the right parser, but only returns the 
    /// left parser's matches.</returns>
    public static IParser<TSource, TResult> IgnoreTrailing<TSource, TIgnore, TResult>(
      this IParser<TSource, TResult> parser,
      IParser<TSource, TIgnore> ignoreParser)
    {
      Contract.Requires(parser != null);
      Contract.Requires(ignoreParser != null);
      Contract.Ensures(Contract.Result<IParser<TSource, TResult>>() != null);

      return parser.SelectMany(_ => ignoreParser, (leading, _) => leading);
    }

    /// <summary>
    /// Matches all results from the specified <paramref name="parser"/> that equal the specified
    /// <paramref name="value"/>.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <param name="parser">The parser from which matches equivalent to the specified <paramref name="value"/> will be yielded.</param>
    /// <param name="value">The value to be compared to matches for equality.</param>
    /// <returns>A parser that matches only those results from the specified <paramref name="parser"/> that equal
    /// the specified <paramref name="value"/>.</returns>
    public static IParser<TSource, TResult> Of<TSource, TResult>(
      this IParser<TSource, TResult> parser,
      TResult value)
    {
      Contract.Requires(parser != null);
      Contract.Ensures(Contract.Result<IParser<TSource, TResult>>() != null);

      return parser.Of(value, EqualityComparer<TResult>.Default);
    }

    /// <summary>
    /// Matches all results from the specified <paramref name="parser"/> that equal the specified
    /// <paramref name="value"/> using the specified <paramref name="comparer"/>.
    /// </summary>
    /// <typeparam name="TSource">The type of the source elements.</typeparam>
    /// <typeparam name="TResult">The type of the elements that are generated from parsing the source elements.</typeparam>
    /// <param name="parser">The parser from which matches equivalent to the specified <paramref name="value"/> will be yielded.</param>
    /// <param name="value">The value to be compared to matches for equality.</param>
    /// <param name="comparer">The object that compares matches to the specified <paramref name="value"/> for equality.</param>
    /// <returns>A parser that matches only those results from the specified <paramref name="parser"/> that equal
    /// the specified <paramref name="value"/> using the specified <paramref name="comparer"/>.</returns>
    public static IParser<TSource, TResult> Of<TSource, TResult>(
      this IParser<TSource, TResult> parser,
      TResult value,
      IEqualityComparer<TResult> comparer)
    {
      Contract.Requires(parser != null);
      Contract.Requires(comparer != null);
      Contract.Ensures(Contract.Result<IParser<TSource, TResult>>() != null);

      return parser.Where(v => comparer.Equals(v, value));
    }
  }
}
