# Formula1Database

## Summary of the Code

### Purpose:
The code is designed to store, retrieve, and manage data related to Formula 1, including drivers, teams, races, and race results for the 2024 season. The custom database uses block storage and B-Tree indexing for efficient data handling and querying, making it suitable for managing large datasets over multiple years.

### Overview:

#### Initialization of the Database:
The Formula1Database class initializes a database with given storage and indexing structures, allowing for flexibility in data operations.

#### Data Models:
- **DriverModel**: Stores information about drivers, including ID, name, age, nationality, and profile picture.
- **TeamModel**: Stores information about teams, including ID, name, base location, and logo.
- **RaceModel**: Stores information about races, including ID, name, date, and location.
- **ResultModel**: Stores information about race results, including ID, race ID, driver ID, team ID, position, and time.

#### Block Storage:
BlockStorage initializes with a stream and block size, ensuring data is stored in blocks for efficient access and modification.

#### Record Storage:
RecordStorage handles the storage of variable-length records using the block storage system, supporting the creation, updating, finding, and deleting of records.

#### B-Tree Indexing:
BTree class implements B-Tree indexing to manage and query data efficiently, allowing for quick lookup and modification of records.

### Detailed Functionality:

#### Formula1Database Class:

- **Initialization**:
  - The constructor sets up record storage and initializes B-Tree indexes for drivers, teams, races, and results.

- **Data Operations**:
  - `InsertDriver`, `InsertTeam`, `InsertRace`, `InsertResult`: Insert new records into the database.
  - `DeleteDriver`, `DeleteTeam`, `DeleteRace`, `DeleteResult`: Delete records from the database.
  - `UpdateDriver`, `UpdateTeam`, `UpdateRace`, `UpdateResult`: Update existing records in the database.
  - `FindDriver`, `FindTeam`, `FindRace`, `FindResult`: Retrieve records by their ID.
  - `FindDriversByAge`: Retrieve drivers by their age.
  - `FindResultsByRace`, `FindResultsByDriver`: Retrieve results by race ID or driver ID.

#### BlockStorage Class:
- Initializes a stream with block size, ensuring the stream length is a multiple of the block size.
- Methods for creating new blocks, finding existing blocks, and reading/writing block data.

#### RecordStorage Class:
- Manages the creation, updating, finding, and deletion of records using block storage.
- Handles variable-length data by linking multiple blocks together.

#### BTree Class:
- Implements B-Tree for efficient data indexing and querying.
- Methods for inserting, finding, and deleting keys and values.

### Example Usage:

Upon running the project, it will insert data for the current F1 drivers, teams, and races for the 2024 season. It will also insert some placeholder race results. The application will then retrieve and display the inserted data.

### Inserting Driver Images:

Driver images can be stored as byte arrays.

Yearly Updates:
The database can be used year by year to maintain a historical record of Formula 1 seasons. This approach is beneficial for:

-Historical Analysis: Tracking performance over multiple seasons.
-Data Integrity: Maintaining consistent and organized data.
-Efficiency: Efficient querying and data retrieval with custom storage and indexing.
-Scalability: Scales efficiently with additional data each year.

### Running the Project

To run the project and see the database in action after loading all the data (drivers, teams, races, and results), use the following commands:

1. Navigate to the project directory:
### Running the Project

To run the project and see the database in action after loading all the data (drivers, teams, races, and results), use the following commands in the terminal:

1. Navigate to the project directory:

```
1. cd /path/to/your/Formula1Database
2. dotnet restore
3. dotnet build
4. dotnet run
