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
- 
## TODOs:
- [ ] missing interfaces, most services are directly registered

## Dependencies ...
- Autofac
- newtonsoft.json
- Rx.Net