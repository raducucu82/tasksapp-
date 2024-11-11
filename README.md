# Tasks

Scaffold for a Tasks App, with persistent undo/redo and throttling support for db save.

## Models
POCO classes for:
- Users (can have many tasks)
- Tasks
- Undo/Redo History

## ModelView/Middle layer
`UserSession`: caches user tasks locally, handles updates, communication with
persistence layer.

## Persistence Layer
- Simulate long-running save (to cloud) operations.
- Throttling support: client notifies the update, hot method: post to observable and exit 
(wanted to experiment with rx.net). Observer triggers the save.
- Cut in half and split in client-server. Insert REST webapi here.

## CLI test app
- Seed some data and run a happy-path scenario for undo/redo and to check throttling.
### Sample run:
```

[] Seed test data
>> DB Snapshot 11/12/2024 12:02:26 AM
- user1 (:1)
        - Description for Task 1 (:1)
- user2 (:2)
        - Description for Task 1 (:2)
        - Description for Task 2 (:3)
<<END
[] Update a task.
[] Wait for db save.
 -> Start write to DB...
 -> End write to DB.
>> DB Snapshot 11/12/2024 12:02:28 AM
- user1 (:1)
        - Updated description. (:1)
- user2 (:2)
        - Description for Task 1 (:2)
        - Description for Task 2 (:3)
<<END
[] Undo task update and wait
[] Wait for db save.
 -> Start write to DB...
 -> End write to DB.
>> DB Snapshot 11/12/2024 12:02:30 AM
- user1 (:1)
        - Description for Task 1 (:1)
- user2 (:2)
        - Description for Task 1 (:2)
        - Description for Task 2 (:3)
<<END
[] Chain two updates, with enough delay after the first update to trigger a db save.
 -> Start write to DB...
[] First db update trigger should be canceled by the second.
 -> Canceled.
 -> Start write to DB...
 -> End write to DB.
>> DB Snapshot 11/12/2024 12:02:33 AM
- user1 (:1)
        - Overwrite the undo. (:1)
- user2 (:2)
        - Description for Task 1 (:2)
        - Description for Task 2 (:3)
<<END
```

## TODOs:
- [ ] missing interfaces, most services are directly registered

## Dependencies ...
- Autofac
- newtonsoft.json
- Rx.Net