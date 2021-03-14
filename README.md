# skautské klání
stránky pro registraci do turnaje, zadávání výsledků, a čtení průběžných výsledků

celé stránky jsou naprogramovány pomocí ASP.NET Core frameworku

# obecné info
Celý Kód je začátečnický a je v něm spousta děr a bezpečnostních chyb. 

discord bot je spuštěný přes web builder hosting.

celý projekt byl vytvořen za měsíc

# spuštění na lokálním pc
pokud chcete vytvořit vlastní kopii je nutné si vytvořit vlastního dicord bota a přepsat v kódu hardcoded channel id a roles ids a dosadit token,
zároveň je nutné vytvořit vlastní firebase project s firestore a vytvořit v něm následující kolekce:

- CS-GO_users
- LoL_users
- Match
- Team
- activators

program hledá google credential v enviroment variables.
compilace je nejednodušší ve visual studio

