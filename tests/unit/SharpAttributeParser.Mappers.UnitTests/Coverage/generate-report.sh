#!/bin/bash

parent_path=$( cd "$(dirname "${BASH_SOURCE[0]}")" ; pwd -P )

cd "$parent_path"

rm -r -f Reports/Coverage/*

mkdir -p Reports

echo "*" > Reports/.gitignore

dotnet test .. --settings coverage.runsettings

result=$?

if [ $result != 0 ]; then
    exit $result
fi

echo

reportgenerator -reports:`find Reports/Coverage -name *.cobertura.xml` -targetdir:Reports/Coverage -reporttype:Html_Dark --riskHotspotsAnalysisThresholds:metricThresholdForCyclomaticComplexity=15

start Reports/Coverage/index.html

exit 0