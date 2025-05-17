using Microsoft.AspNetCore.Mvc.Rendering;

namespace ProjectManagementSystem.Models.Board
{
    public class ProjectBoardViewModel
    {
        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string ProjectCode { get; set; }
        public List<BoardColumn> Columns { get; set; }
        public string StatusId { get; set; }
        public string SprintId { get; set; }
        public List<SelectListItem> SprintList { get; set; } = [];
        public List<SelectListItem> UserList { get; set; } = [];
        public string SprintInterval { get; set; }
        public Guid TeamId { get; set; }

        public ProjectBoardViewModel()
        {
            Columns = new List<BoardColumn>();
        }

        public ProjectBoardViewModel(bool withSampleData) : this()
        {
            if (!withSampleData) return;

            ProjectName = "Sample Project";
            ProjectCode = "PRJ001";
            Columns = new List<BoardColumn>
            {
                new BoardColumn("1", "To Do", new List<BoardTask>
                {
                    new BoardTask
                    {
                        Id = "1",
                        TaskCode = "TASK001",
                        Title = "Design UI",
                        Description = "Design the user interface for the main dashboard",
                        Priority = 1,
                        IsUrgent = true,
                        Subtasks = new List<BoardSubtask>
                        {
                            new BoardSubtask("1", "Create wireframes", false),
                            new BoardSubtask("2", "Design color scheme", true)
                        },
                        Assigned = new Assignee("1", "Alice", "/images/alice.png")
                    },
                    new BoardTask
                    {
                        Id = "2",
                        TaskCode = "TASK002",
                        Title = "Set up DB",
                        Description = "Initialize database schema",
                        Priority = 2,
                        IsUrgent = false,
                        Subtasks = new List<BoardSubtask>(),
                        Assigned = new Assignee("2", "Bob", "/images/bob.png")
                    }
                }),
                new BoardColumn("2", "In Progress", new List<BoardTask>
                {
                    new BoardTask
                    {
                        Id = "3",
                        TaskCode = "TASK003",
                        Title = "Implement API",
                        Description = "Create endpoints for task management",
                        Priority = 1,
                        IsUrgent = true,
                        Subtasks = new List<BoardSubtask>
                        {
                            new BoardSubtask("3", "Create Task controller", false),
                            new BoardSubtask("4", "Implement GET endpoints", true),
                            new BoardSubtask("5", "Implement POST endpoints", false)
                        },
                        Assigned = new Assignee("3", "Charlie", "/images/charlie.png")
                    }
                }),
                new BoardColumn("3", "Done", new List<BoardTask>()),
            };
        }
    }

    public class BoardColumn
    {
        public string StatusId { get; set; }
        public string Title { get; set; }
        public List<BoardTask> Tasks { get; set; }

        public BoardColumn()
        {
            Tasks = new List<BoardTask>();
        }

        public BoardColumn(string statusId, string title, List<BoardTask> tasks) : this()
        {
            StatusId = statusId;
            Title = title;
            Tasks = tasks ?? new List<BoardTask>();
        }
    }

    public class BoardTask
    {
        public string Id { get; set; }
        public string TaskCode { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Priority { get; set; } // 1: High, 2: Medium, 3: Low
        public bool IsUrgent { get; set; }
        public List<BoardSubtask> Subtasks { get; set; }
        public string UserId { get; set; }
        public Assignee Assigned { get; set; }

        public int SubtaskCount => Subtasks?.Count ?? 0;
        public int CompletedSubtasks => Subtasks?.Count(s => s.IsCompleted) ?? 0;
        public int SubtaskCompletionPercentage => SubtaskCount == 0 ? 0 : (int)((CompletedSubtasks / (double)SubtaskCount) * 100);

        public BoardTask()
        {
            Subtasks = new List<BoardSubtask>();
            Assigned = new Assignee();
        }
    }

    public class BoardSubtask
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public bool IsCompleted { get; set; }

        public BoardSubtask() { }

        public BoardSubtask(string id, string title, bool isCompleted)
        {
            Id = id;
            Title = title;
            IsCompleted = isCompleted;
        }
    }

    public class Assignee
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string AvatarUrl { get; set; }

        public Assignee() { }

        public Assignee(string id, string name, string avatarUrl)
        {
            Id = id;
            Name = name;
            AvatarUrl = avatarUrl;
        }
    }
}