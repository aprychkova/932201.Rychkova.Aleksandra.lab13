using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

public class QuizController : Controller
{
    private const string SessionKey = "QuizState";

    private static readonly List<QuizModel> DefaultQuestions = new()
    {
        new QuizModel { QuestionNumber = 1, Question = "1 - 6", CorrectAnswer = -5 },
        new QuizModel { QuestionNumber = 2, Question = "8 + 6", CorrectAnswer = 14 },
        new QuizModel { QuestionNumber = 3, Question = "5 - 7", CorrectAnswer = -2 },
        new QuizModel { QuestionNumber = 4, Question = "5 - 2", CorrectAnswer = 3 }
    };

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Quiz(int id = 1)
    {
        var questions = GetQuestionsFromSession();

        if (id > questions.Count)
            return RedirectToAction("Results");

        var question = questions[id - 1];
        return View(question);
    }

    [HttpPost]
    public IActionResult Quiz(int questionNumber, int userAnswer, string action)
    {
        var questions = GetQuestionsFromSession();
        var question = questions.First(q => q.QuestionNumber == questionNumber);

        if (!ModelState.IsValid)
        {
            return View(question);
        }

        question.UserAnswer = userAnswer;
        SaveQuestionsToSession(questions);

        if (action == "Finish")
        {
            return RedirectToAction("Results");
        }

        if (questionNumber < questions.Count)
        {
            return RedirectToAction("Quiz", new { id = questionNumber + 1 });
        }

        return RedirectToAction("Results");
    }

    public IActionResult Results()
    {
        var questions = GetQuestionsFromSession();
        var results = new QuizResults
        {
            Questions = questions,
            CorrectAnswers = questions.Count(q => q.UserAnswer == q.CorrectAnswer)
        };

        return View(results);
    }

    private List<QuizModel> GetQuestionsFromSession()
    {
        if (HttpContext.Session.TryGetValue(SessionKey, out var data))
        {
            return System.Text.Json.JsonSerializer.Deserialize<List<QuizModel>>(data);
        }

        return DefaultQuestions;
    }

    private void SaveQuestionsToSession(List<QuizModel> questions)
    {
        var data = System.Text.Json.JsonSerializer.Serialize(questions);
        HttpContext.Session.Set(SessionKey, System.Text.Encoding.UTF8.GetBytes(data));
    }
}