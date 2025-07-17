using SecurityPlusPractice.Data;
using SecurityPlusPractice.Models;

namespace SecurityPlusPractice.Services
{
	public static class Seeder
	{
		public static void SeedData(AppDbContext context, IWebHostEnvironment env)
		{
			if (context.Questions.Any()) return;

			var questionsFile = "./Data/SeedFiles/questions_10k.txt";
			var choicesFile = "./Data/SeedFiles/choices_10k.txt";
			var answersFile = "./Data/SeedFiles/answers_10k.txt";
			var explanationsFile = "./Data/SeedFiles/explanations_10k.txt";

			if (!File.Exists(questionsFile) || !File.Exists(choicesFile) ||
				!File.Exists(answersFile) || !File.Exists(explanationsFile))
			{
				throw new FileNotFoundException("One or more seed files are missing.");
			}

			var questionLines = File.ReadAllLines(questionsFile).Skip(1);
			var choiceLines = File.ReadAllLines(choicesFile).Skip(1);
			var answerLines = File.ReadAllLines(answersFile).Skip(1);
			var explanationLines = File.ReadAllLines(explanationsFile).Skip(1);

			// Create questions first and save to get IDs
			var questions = questionLines
				.Select(line => line.Split('\t'))
				.Where(parts => parts.Length >= 2)
				.Select(parts => new Question { QuestionText = parts[1].Trim() })
				.ToList();

			context.Questions.AddRange(questions);
			context.SaveChanges();

			// Now attach related data by index match
			var choices = new List<Choice>();
			var answers = new List<Answer>();
			var explanations = new List<Explanation>();

			for (int i = 0; i < questions.Count; i++)
			{
				var question = questions[i];

				// Choices
				if (i < choiceLines.Count())
				{
					var parts = choiceLines.ElementAt(i).Split('\t');
					if (parts.Length >= 5)
					{
						choices.Add(new Choice
						{
							ID = question.ID,
							ChoiceA = parts[1].Trim(),
							ChoiceB = parts[2].Trim(),
							ChoiceC = parts[3].Trim(),
							ChoiceD = parts[4].Trim()
						});
					}
				}

				// Answer
				if (i < answerLines.Count())
				{
					var parts = answerLines.ElementAt(i).Split('\t');
					if (parts.Length >= 2)
					{
						answers.Add(new Answer
						{
							ID = question.ID,
							CorrectAnswer = parts[1].Trim()
						});
					}
				}

				// Explanation
				if (i < explanationLines.Count())
				{
					var parts = explanationLines.ElementAt(i).Split('\t');
					if (parts.Length >= 2)
					{
						explanations.Add(new Explanation
						{
							ID = question.ID,
							ExplanationText = parts[1].Trim()
						});
					}
				}
			}

			context.Choices.AddRange(choices);
			context.Answers.AddRange(answers);
			context.Explanations.AddRange(explanations);
			context.SaveChanges();
		}
	}
}
