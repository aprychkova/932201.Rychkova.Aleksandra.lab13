using System.ComponentModel.DataAnnotations;

public class QuizModel
{
    public int QuestionNumber { get; set; }
    public string? Question { get; set; }

    [Required(ErrorMessage = "Ответ обязателен")]
    public int? UserAnswer { get; set; }

    public int CorrectAnswer { get; set; }
}

public class QuizResults
{
    public List<QuizModel> Questions { get; set; } = new();
    public int CorrectAnswers { get; set; }
}