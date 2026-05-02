# ClassRoom Control 

Classroom management system built by our Team 4 Inspired by NetSupport School.

## 👥 Team Members

- Ahmed Maged Motea Yousif  
- Ahmed Yasser Shehata Sultan  
- Anas Mohamed Mohamed Ahmed  
- Saher Ayman Munir Hassaballh  

## 3 Apps

| App | What it does |
|-----|-------------|
| **Instructor** | Controls learners, sends lock/unlock, starts exams, tracks progress |
| **Learner** | Joins classroom, takes exams, shows lock screen |
| **ExamBuilder** | Creates MCQ exams and saves as JSON |

## Tech
- C# / .NET 10 / WinForms
- File-based JSON communication (no server needed)
- Arabic RTL support

## Run

```bash
# 3 separate terminals:
dotnet run --project src/Name_folder.Instructor
dotnet run --project src/Name_folder.Learner
dotnet run --project src/Name_folder.ExamBuilder
