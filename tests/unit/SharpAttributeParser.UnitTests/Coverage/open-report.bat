#!/bin/sh

parent_path=$( cd "$(dirname "${BASH_SOURCE[0]}")" ; pwd -P )

cd "$parent_path"

FILE=Reports/Coverage/index.html

if [ ! -f "$FILE" ]; then
    echo "No report has been generated. Run generate-report first."
    exit 1
fi

start Reports/Coverage/index.html

exit 0