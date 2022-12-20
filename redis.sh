redis-server --daemonize yes && sleep 3
redis-cli MSET config-amorphie-transaction-db 'User ID=postgres;Password=qwerty123;Host=postgre;Port=5432;Database=transactions;'
redis-cli save 
redis-cli shutdown 
redis-server

  