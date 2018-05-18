echo off

set project_name=DLC.Framework
..\.nuget\nuget.exe pack %project_name%.csproj -Properties Configuration=Release -Build -Symbols -IncludeReferencedProjects -NonInteractive