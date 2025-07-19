using Microsoft.AspNetCore.Mvc;
using SecurityPlusPractice.Data;
using SecurityPlusPractice.Models;
using SecurityPlusPractice.ViewModels;

namespace SecurityPlusPractice.Controllers
{
    public class SessionController : Controller
    {
        private readonly AppDbContext _dbContext;
        private static readonly Random _random = new();

        public SessionController(AppDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        [HttpGet]
        public IActionResult Start()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Begin()
        {
            var user = Environment.UserName;
            var total = _random.Next(74, 91);
            var questions = _dbContext.Questions
                .OrderBy(q => Guid.NewGuid())
                .Take(total)
                .ToList();

            var session = new Session
            {
                UserName = user,
                StartedAt = DateTime.Now,
                TotalQuestions = total
            };

            _dbContext.Sessions.Add(session);
            _dbContext.SaveChanges();

            TempData["SessionID"] = session.SessionID;
            TempData["QuestionIDs"] = string.Join(",", questions.Select(q => q.ID));

            return RedirectToAction("Question", new { index = 0 });
        }

        [HttpGet]
        public IActionResult Question(int index)
        {
            if (TempData["SessionID"] == null || TempData["QuestionIDs"] == null)
            {
                return RedirectToAction("Start");
            }

            int sessionId = Convert.ToInt32(TempData["SessionID"]);
            string[] idStrings = TempData["QuestionIDs"].ToString()!.Split(',');
            List<int> questionIds = idStrings.Select(int.Parse).ToList();

            if (index >= questionIds.Count)
            {
                return RedirectToAction("Finish");
            }

            var question = _dbContext.Questions.FirstOrDefault(q => q.ID == questionIds[index]);
            var choices = _dbContext.Choices.FirstOrDefault(c => c.ID == questionIds[index]);

            if (question == null || choices == null)
            {
                return NotFound();
            }

            TempData.Keep("SessionID");
            TempData.Keep("QuestionIDs");

            var viewModel = new QuestionViewModel
            {
                QuestionID = question.ID,
                QuestionText = question.QuestionText,
                Choices = new Dictionary<string, string>
                {
                    { "A", choices.ChoiceA },
                    { "B", choices.ChoiceB },
                    { "C", choices.ChoiceC },
                    { "D", choices.ChoiceD }
                },
                CurrentIndex = index,
                TotalQuestions = questionIds.Count
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult SubmitAnswer(int questionID, string[] selectedAnswers)
        {
            if (!TempData.TryGetValue("SessionID", out var sessionIdObj) ||
                !TempData.TryGetValue("QuestionIDs", out var questionIdsObj))
            {
                return RedirectToAction("Start");
            }

            int sessionID = Convert.ToInt32(sessionIdObj);
            var questionIDs = questionIdsObj.ToString()?.Split(',').Select(int.Parse).ToList() ?? new List<int>();

            var question = _dbContext.Questions.SingleOrDefault(q => q.ID == questionID);
            var answer = _dbContext.Answers.SingleOrDefault(a => a.ID == questionID);
            var explanation = _dbContext.Explanations.SingleOrDefault(e => e.ID == questionID);
            var choice = _dbContext.Choices.SingleOrDefault(c => c.ID == questionID);

            // Normalize and sort answers for multi-answer comparison
            var selected = selectedAnswers.Select(a => a.Trim().ToUpper()).OrderBy(a => a).ToList();
            var correct = (answer?.CorrectAnswer ?? "")
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(a => a.Trim().ToUpper())
                .OrderBy(a => a)
                .ToList();

            bool isCorrect = selected.SequenceEqual(correct);

            _dbContext.SessionAnswers.Add(new SessionAnswer
            {
                SessionID = sessionID,
                QuestionID = questionID,
                UserName = Environment.UserName,
                SelectedAnswer = string.Join(",", selected),
                IsCorrect = isCorrect,
                AnsweredAt = DateTime.Now
            });
            _dbContext.SaveChanges();

            var choicesDict = new Dictionary<string, string>();
            if (choice != null)
            {
                choicesDict["A"] = choice.ChoiceA;
                choicesDict["B"] = choice.ChoiceB;
                choicesDict["C"] = choice.ChoiceC;
                choicesDict["D"] = choice.ChoiceD;
            }

            var viewModel = new AnswerReviewViewModel
            {
                QuestionText = question?.QuestionText ?? "Unknown Question",
                Choices = choicesDict,
                SelectedAnswer = string.Join(",", selected),
                CorrectAnswer = string.Join(",", correct),
                IsCorrect = isCorrect,
                Explanation = explanation?.ExplanationText ?? "No explanation available.",
                NextIndex = questionIDs.IndexOf(questionID) + 1
            };

            TempData["SessionID"] = sessionID;
            TempData["QuestionIDs"] = string.Join(",", questionIDs);

            return View("Review", viewModel);
        }

        [HttpGet]
        public IActionResult Finish()
        {
            if (TempData["SessionID"] == null)
                return RedirectToAction("Start");

            int sessionId = Convert.ToInt32(TempData["SessionID"]);
            var session = _dbContext.Sessions.FirstOrDefault(s => s.SessionID == sessionId);
            if (session == null)
                return NotFound();

            session.FinishedAt = DateTime.Now;
            var sessionAnswers = _dbContext.SessionAnswers.Where(sa => sa.SessionID == sessionId).ToList();
            int correct = 0;

            foreach (var sa in sessionAnswers)
            {
                var correctAnswer = _dbContext.Answers.FirstOrDefault(a => a.ID == sa.QuestionID);
                if (correctAnswer == null) continue;

                var selected = (sa.SelectedAnswer ?? "")
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(a => a.Trim().ToUpper())
                    .OrderBy(a => a)
                    .ToList();

                var correctList = (correctAnswer.CorrectAnswer ?? "")
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(a => a.Trim().ToUpper())
                    .OrderBy(a => a)
                    .ToList();

                if (selected.SequenceEqual(correctList))
                {
                    correct++;
                }
            }

            session.CorrectAnswers = correct;
            session.ScorePercent = Math.Round((decimal)correct / session.TotalQuestions * 100, 2);
            _dbContext.SaveChanges();

            return View("Result", new ResultViewModel
            {
                SessionDate = session.StartedAt,
                Score = correct,
                Total = session.TotalQuestions,
                UserName = session.UserName
            });
        }
    }
}

//using Microsoft.AspNetCore.Mvc;
//using SecurityPlusPractice.Data;
//using SecurityPlusPractice.Models;
//using SecurityPlusPractice.ViewModels;

//namespace SecurityPlusPractice.Controllers
//{
//	public class SessionController : Controller
//	{
//		private readonly AppDbContext _dbContext;
//		private static readonly Random _random = new();

//		public SessionController(AppDbContext dbContext)
//		{
//			_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
//		}

//		[HttpGet]
//		public IActionResult Start()
//		{
//			return View();
//		}

//		[HttpPost]
//		public IActionResult Begin()
//		{
//			var user = Environment.UserName;
//			var total = _random.Next(74, 91);
//			var questions = _dbContext.Questions
//				.OrderBy(q => Guid.NewGuid())
//				.Take(total)
//				.ToList();

//			var session = new Session
//			{
//				UserName = user,
//				StartedAt = DateTime.Now,
//				TotalQuestions = total
//			};

//			_dbContext.Sessions.Add(session);
//			_dbContext.SaveChanges();

//			TempData["SessionID"] = session.SessionID;
//			TempData["QuestionIDs"] = string.Join(",", questions.Select(q => q.ID));

//			return RedirectToAction("Question", new { index = 0 });
//		}
//		[HttpGet]
//		public IActionResult Question(int index)
//		{
//			if (TempData["SessionID"] == null || TempData["QuestionIDs"] == null)
//			{
//				return RedirectToAction("Start");
//			}

//			int sessionId = Convert.ToInt32(TempData["SessionID"]);
//			string[] idStrings = TempData["QuestionIDs"].ToString()!.Split(',');
//			List<int> questionIds = idStrings.Select(int.Parse).ToList();

//			if (index >= questionIds.Count)
//			{
//				return RedirectToAction("Finish");
//			}

//			var question = _dbContext.Questions.FirstOrDefault(q => q.ID == questionIds[index]);
//			var choices = _dbContext.Choices.FirstOrDefault(c => c.ID == questionIds[index]);

//			if (question == null || choices == null)
//			{
//				return NotFound();
//			}

//			TempData.Keep("SessionID");
//			TempData.Keep("QuestionIDs");

//			var viewModel = new QuestionViewModel
//			{
//				QuestionID = question.ID,
//				QuestionText = question.QuestionText,
//				Choices = new Dictionary<string, string>
//				{
//					{ "A", choices.ChoiceA },
//					{ "B", choices.ChoiceB },
//					{ "C", choices.ChoiceC },
//					{ "D", choices.ChoiceD }
//				},
//				CurrentIndex = index,
//				TotalQuestions = questionIds.Count
//			};

//			return View(viewModel);
//		}

//        [HttpPost]
//        public IActionResult SubmitAnswer(int questionID, string[] selectedAnswers)
//        {
//            if (!TempData.TryGetValue("SessionID", out var sessionIdObj) ||
//                !TempData.TryGetValue("QuestionIDs", out var questionIdsObj))
//            {
//                return RedirectToAction("Start");
//            }

//            int sessionID = Convert.ToInt32(sessionIdObj);
//            var questionIDs = questionIdsObj.ToString()?.Split(',').Select(int.Parse).ToList() ?? new List<int>();

//            var question = _dbContext.Questions.SingleOrDefault(q => q.ID == questionID);
//            var answer = _dbContext.Answers.SingleOrDefault(a => a.ID == questionID);
//            var explanation = _dbContext.Explanations.SingleOrDefault(e => e.ID == questionID);
//            var choice = _dbContext.Choices.SingleOrDefault(c => c.ID == questionID);

//            bool isCorrect = answer != null && selectedAnswers.Length == 1 && answer.CorrectAnswer == selectedAnswers[0];

//            _dbContext.SessionAnswers.Add(new SessionAnswer
//            {
//                SessionID = sessionID,
//                QuestionID = questionID,
//                UserName = Environment.UserName,
//                SelectedAnswer = string.Join(",", selectedAnswers),
//                IsCorrect = isCorrect,
//                AnsweredAt = DateTime.Now
//            });
//            _dbContext.SaveChanges();

//            // Build Choices Dictionary safely
//            var choicesDict = new Dictionary<string, string>();
//            if (choice != null)
//            {
//                choicesDict["A"] = choice.ChoiceA;
//                choicesDict["B"] = choice.ChoiceB;
//                choicesDict["C"] = choice.ChoiceC;
//                choicesDict["D"] = choice.ChoiceD;
//            }

//            var viewModel = new AnswerReviewViewModel
//            {
//                QuestionText = question?.QuestionText ?? "Unknown Question",
//                Choices = choicesDict,
//                SelectedAnswer = selectedAnswers.FirstOrDefault() ?? "",
//                CorrectAnswer = answer?.CorrectAnswer ?? "",
//                IsCorrect = isCorrect,
//                Explanation = explanation?.ExplanationText ?? "No explanation available.",
//                NextIndex = questionIDs.IndexOf(questionID) + 1
//            };

//            TempData["SessionID"] = sessionID;
//            TempData["QuestionIDs"] = string.Join(",", questionIDs);

//            return View("Review", viewModel);
//        }

//        [HttpGet]
//        public IActionResult Finish()
//        {
//            if (TempData["SessionID"] == null)
//                return RedirectToAction("Start");

//            int sessionId = Convert.ToInt32(TempData["SessionID"]);

//            var session = _dbContext.Sessions.FirstOrDefault(s => s.SessionID == sessionId);
//            if (session == null)
//                return NotFound();

//            session.FinishedAt = DateTime.Now;
//            var sessionAnswers = _dbContext.SessionAnswers.Where(sa => sa.SessionID == sessionId).ToList();

//            // Evaluate score
//            int correct = 0;
//            foreach (var sa in sessionAnswers)
//            {
//                var correctAnswer = _dbContext.Answers.FirstOrDefault(a => a.ID == sa.QuestionID);
//                if (correctAnswer != null && correctAnswer.CorrectAnswer == sa.SelectedAnswer)
//                    correct++;
//            }

//            session.CorrectAnswers = correct;
//            session.ScorePercent = Math.Round((decimal)correct / session.TotalQuestions * 100, 2);

//            _dbContext.SaveChanges();

//            return View("Result", new ResultViewModel
//            {
//                SessionDate = session.StartedAt,
//                Score = correct,
//                Total = session.TotalQuestions,
//                UserName = session.UserName
//            });
//        }


//    }
//}
