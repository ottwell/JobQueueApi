### Jobs Queue Challenge API

A POC solution built using .NET 5. Database simulated using Entity Framework core in-memory functionality. Unit tests implemented using XUNIT.
Queue functionality implemented using ASP.NET Core hosted services.

##### API Description
- GET `/api/jobs`. Returns all jobs in the database.
- GET `/api/jobs/{GUID}`. Returns the job with the corresponding Id value. If said job has 'Queued' status, the return object will include its' current place in the queue.
- POST `/api/jobs`. Creates a new job entity, inserts it to the database and queues the job for processing. The request body must be an array of ints (Example: [1,2,3,4,5]). Returns a 201 response, with the created object and its' current place in queue.
- PUT `/api/jobs/{GUID}`. Updates an existing job entity and queues it for processing. The request body must be an array of ints (Example: [1,2,3,4,5]). Returns a 202 response, with the updated object and its' current place in queue.


##### Job Processing
The input array is sorted in ascending order and saved in the database. The job status is changed to: Completed.
In order to simulate a background task, there is an artificial delay in execution. The length of the delay is controlled by a configuration parameter.

##### Building
1. Clone or download the repository.
2. Run `dotnet build`.

##### Running locally
1. Clone or download the repository.
2. Run `cd JobQueueApi`
3. Run `dotnet run`.
4. The server is started and listening at https://localhost:44369.

##### Testing
1. Clone or download the repository.
2. Run `dotnet test`.

##### Logging
All logs are saved in a rolling file located in `\JobQueueApi\Logs`.

