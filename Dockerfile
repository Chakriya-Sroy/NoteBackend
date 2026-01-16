FROM mcr.microsoft.com/mssql/server:2022-latest

# Set environment variables
ENV ACCEPT_EULA=Y
ENV MSSQL_SA_PASSWORD=YayaNetPassw0rd

# Switch to root to ensure we can copy files
USER root

# Create the directory and copy the script
RUN mkdir -p /usr/config
COPY init-db.sql /usr/config/init-db.sql

# Give the mssql user permission to the config folder
RUN chown -R mssql /usr/config

# Switch back to the mssql user for security
USER mssql

# Expose port
EXPOSE 1433

# Start SQL Server
CMD ["/opt/mssql/bin/sqlservr"]