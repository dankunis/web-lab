#!/bin/bash

java -jar ../../liquibase/liquibase.jar  --changeLogFile=../../Homewrok1/src/Database/Migrations/index.xml \
--username=postgres \
--password=0000 \
--url=jdbc:postgresql://localhost:5432/homework1 \
--driver=org.postgresql.Driver \
--classpath=../../postgresql-42.2.5.jar \
--contexts="test" \
update