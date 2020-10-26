FROM liquibase

COPY ./Homework1/src/Database/Migrations ./Migrations
COPY ./run-migrations.sh ./
RUN chmod 777 ./run-migrations.sh
ENTRYPOINT ./run-migrations.sh