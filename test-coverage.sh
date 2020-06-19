#!/bin/bash
# run and report xml file
dotnet test --collect:"XPlat Code Coverage"

# copy ./BlazeSnes.Core.Test/TestResults/{hash}/coverage.cobertura.xml
find ./BlazeSnes.Core.Test/TestResults/ | tail -n 1 | xargs -I "{}" cp "{}" ./

# create report to ./coverage
reportgenerator "-reports:coverage.cobertura.xml" "-targetdir:coverage" "-reporttypes:Html;Badges"