# School Database Management System

This project is a **Minimum Viable Product (MVP)** for managing the **Teachers** table in a school database using **ASP.NET Core Web API and MVC**. It provides full **CRUD (Create, Read, Update, Delete)** functionality for managing teachers efficiently.

---

## Features

### Teacher Management
- List all teachers with search functionality (by name, hire date, or salary)
- View detailed teacher information
- Add new teachers with validation
- Update teacher information with client/server-side validation
- Delete teachers with confirmation
- Advanced search filtering by name, hire date range, or salary

### Database Integration
- Uses **MySQL** as the database
- **Entity Framework** for database access and management
- API endpoints for handling data operations

### Web Pages
- `/TeacherPage/List` - Displays a list of all teachers with a search bar  
- `/TeacherPage/Show/{id}` - Displays detailed information about a specific teacher  
- `/TeacherPage/Edit/{id}` - Edit an existing teacher's info with form validation  
- `/TeacherPage/New` - Add a new teacher  
- `/TeacherPage/DeleteConfirm/{id}` - Confirm before deleting a teacher

---

## API Endpoints

- `GET /api/TeacherData/ListTeachers`  
  List all teachers

- `GET /api/TeacherData/FindTeacher/{id}`  
  Find a teacher by ID

- `GET /api/TeacherData/ListTeachersAdvanced?searchKey=...&minDate=...&maxDate=...`  
  Advanced teacher search by name, hire date, or salary

- `POST /api/TeacherData/AddTeacher`  
  Add a new teacher

- `PUT /api/TeacherData/UpdateTeacher/{id}`  
  Update a teacher’s information

- `DELETE /api/TeacherData/DeleteTeacher/{id}`  
  Delete a teacher

---

## Validation Rules

### ✅ Server & Client-Side Validations
- **Teacher names** cannot be empty
- **Hire date** cannot be in the future
- **Salary** cannot be negative
- **Employee number** must be in the format `"T"` followed by digits (e.g., `T123`)
- **Employee number** must be unique
- **Non-existent teacher ID** returns `404 Not Found`

---

## cURL Testing Commands

### 🔍 List All Teachers
```bash
curl -k "https://localhost:44383/api/TeacherData/ListTeachers"
