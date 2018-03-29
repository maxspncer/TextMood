using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

using TextMood.Backend.Common;
using TextMood.Shared;

namespace TextMood.Functions.Functions
{
    [StorageAccount(QueueNameConstants.AzureWebJobsStorage)]
    public static class UpdatePhillipsHueLight
    {
        [FunctionName(nameof(UpdatePhillipsHueLight))]
        public static async Task Run([QueueTrigger(QueueNameConstants.UpdatePhillipsHueLight)]string message, TraceWriter log)
        {
            log.Info("Retrieving All Text Models From Database");

            var allTextModelsFromDatabase = await TextMoodDatabase.GetAllTextModels().ConfigureAwait(false);
            log.Info($"Retrived {allTextModelsFromDatabase?.Count ?? -1} Text Models");
            
            log.Info($"Current Utc Time: {DateTimeOffset.UtcNow}");
            
            log.Info("Retrieving Text Models From Past Hour");
			var textModelsFromPastHour = TextModelServices.GetRecentTextModels(new List<ITextMoodModel>(allTextModelsFromDatabase), TimeSpan.FromHours(1));
            log.Info($"Retrived {textModelsFromPastHour?.Count ?? -1} Text Models");


			log.Info("Get Average Sentiment Score");
            var averageSentiment = TextModelServices.GetAverageSentimentScore(textModelsFromPastHour);

            log.Info($"One Hour Running Sentiment Average: {averageSentiment}");
        }
    }
}