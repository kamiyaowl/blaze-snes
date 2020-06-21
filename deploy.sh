#!/bin/bash
# build publish files
dotnet publish -c Release

# run and report xml file
dotnet test --collect:"XPlat Code Coverage" --logger "junit;"
# copy ./BlazeSnes.Core.Test/TestResults/{hash}/coverage.cobertura.xml
find ./BlazeSnes.Core.Test/TestResults/ | tail -n 1 | xargs -I "{}" cp "{}" ./
# create code coverage report to ./coverage
reportgenerator "-reports:coverage.cobertura.xml" "-targetdir:coverage" "-reporttypes:Html;Badges"
# copy coverage reports to publish directory
cp -r ./coverage ./BlazeSnes/bin/Release/netstandard2.1/publish/wwwroot/

# crate unit test report
mkdir -p ./unittest
junit2html BlazeSnes.Test/TestResults/TestResults.xml ./unittest/blazesnes.html
junit2html BlazeSnes.Core.Test/TestResults/TestResults.xml ./unittest/blazesnes.core.html
# copy unit test reports to publish directory
cp -r ./unittest ./BlazeSnes/bin/Release/netstandard2.1/publish/wwwroot/
