# School Database Management System

This project is a **Minimum Viable Product (MVP)** for managing the **Teachers** table in a school database using **ASP.NET Core Web API and MVC**. It provides full **CRUD (Create, Read, Update, Delete)** functionality for managing teachers efficiently.

## Features

### Teacher Management
- List all teachers with search functionality (by name, hire date, or salary)
- View detailed teacher information
- Add new teachers with validation
- Delete teachers with confirmation
- Search teachers by hire date range

### Database Integration
- Uses **MySQL** as the database
- **Entity Framework** for database access and management
- API endpoints for handling data operations

### Web Pages
- `/TeacherPage/List` - Displays a list of all teachers with a search bar  
- `/TeacherPage/Show/{id}` - Displays detailed information about a specific teacher  

## API Endpoints

- `GET /api/TeacherData/ListTeachers` - List all teachers  
- `GET /api/TeacherData/FindTeacher/{id}` - Find a teacher by ID  
- `POST /api/TeacherData/AddTeacher` - Add a new teacher  
- `DELETE /api/TeacherData/DeleteTeacher/{id}` - Delete a teacher  

## Validation Rules

- **Teacher names** cannot be empty
- **Hire date** cannot be in the future
- **Employee number** must be in the format `"T"` followed by digits (e.g., `T123`)
- **Employee number** must be unique
