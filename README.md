# Design of HA-entended model [![Build status](https://ci.appveyor.com/api/projects/status/37ngnmo4eibw42rg/branch/develop?svg=true)](https://ci.appveyor.com/project/amat27/ha-module/branch/develop) [![codecov](https://codecov.io/gh/amat27/ha-module/branch/develop/graph/badge.svg)](https://codecov.io/gh/amat27/ha-module)
Algorithms for doing leader election and name resolving with the help of another HA system.

## Parameters
 - `I`: intervalfor heartbeat (e.g. 1 sec)
 - `T`: heartbeat timeout (e.g. 5 secs)
 - `T>2 * I`
 
## Data
 - `Heartbeat table`: A tabel in the SQL always-on instance contains heartbeat entry.
 - `Heartbeat Entry`: in the format `{uuid, utype, timestamp}`
 - `sql_time`: current time in SQL server
 - All time is in UTC time

## Procedures
 - `UpdateHeartBeat (uuid, utype)`:
   
   For each type, update entry `{old_uuid, utype, old_timestamp}` in heartbeat table with `{uuid, utype, sql_time}`.

   For each type, if uuid is not equal to old_uuid, then (`sql_time – old_timestamp > T`) must be satisfied.

   The update process uses optimistic concurrency control. E.g. if the heartbeat entry has been updated before another heartbeat reaches SQL, the later heartbeat is discarded.

 - `GetPrimary (utype)`:

   Return `(uuid, utype)` in heartbeat entry with the correspangding query utype if (`sql_time - timestamps <= T`). Else return empty value.

## Algorithm
1. After a client S started, it genetates a unique instance ID `uid` to indentify itself and marks itself with the exact `utype`, which it will work as in the future.

2. S calls `GetPrimary (utype)` every `I` secs.

3. If `GetPrimary (utype)` returned empty value, S calls `UpdateHeartbeat (uid, utype)`.

4. Continue to call `GetPrimary (utype)` every `I` secs.

    a. If subsequent call to `GetPrimary (utype)` returns `(uid, utype)` generated in 1, S will then work as primary.

    b. If subsequent call to `GetPrimary (utype)` returns a unique ID which is different from `uid` and the same type with `utype` generated in 1, go back to 2.

    c. If subsequent call to `GetPrimary (utype)` returns an incorrect result, e.g. a unique ID (whether it's the uid generated in 1 or another unique ID) but another type different from utype, error occurred in 3. Retry 3.

    d. If subsequent call to `GetPrimary (utype)` returns an empty value, error occurred in 3. Retry 3.

    

5. S call `UpdateHeartBeat (uid, utype)` and `GetPrimary (utype)` every `I` sec.

    a. If `GetPrimary (utype)` returns anything except `(uid, utype)`, or didn't return for `(T-I)` secs, exit itself and restart.
