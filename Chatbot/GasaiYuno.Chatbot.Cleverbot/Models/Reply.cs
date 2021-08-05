using Newtonsoft.Json;

namespace GasaiYuno.Chatbot.Cleverbot.Models
{
    internal class Reply
    {
        [JsonProperty("cs")]
        public string State;

        [JsonProperty("interaction_count")]
        public string InteractionCount;

        [JsonProperty("input")]
        public string InputMessage;

        [JsonProperty("input_label")]
        public string InputLabel;

        [JsonProperty("input_id")]
        public string InputId;

        [JsonProperty("filtered_input")]
        public string FilteredInputMessage;

        [JsonProperty("predicted_input")]
        public string PredictedInputMessage;

        [JsonProperty("accuracy")]
        public string Accuracy;

        [JsonProperty("output_label")]
        public string OutputLabel;

        [JsonProperty("output_id")]
        public string OutputId;

        [JsonProperty("output")]
        public string OutputMessage;

        [JsonProperty("conversation_id")]
        public string ConversationId;

        [JsonProperty("errorline")]
        public string ErrorLine;

        [JsonProperty("database_version")]
        public string DatabaseVersion;

        [JsonProperty("software_version")]
        public string SoftwareVersion;

        [JsonProperty("time_taken")]
        public string TimeTaken;

        [JsonProperty("random_number")]
        public string RandomNumber;

        [JsonProperty("time_second")]
        public string TimeSeconds;

        [JsonProperty("time_minute")]
        public string TimeMinutes;

        [JsonProperty("time_hour")]
        public string TimeHours;

        [JsonProperty("time_day_of_week")]
        public string TimeDayOfWeek;

        [JsonProperty("time_month")]
        public string TimeMonth;

        [JsonProperty("time_year")]
        public string TimeYear;

        [JsonProperty("time_started")]
        public string TimeStarted;

        [JsonProperty("time_elapsed")]
        public string TimeElapsed;

        [JsonProperty("reaction")]
        public string Reaction;

        [JsonProperty("reaction_tone")]
        public string ReactionTone;

        [JsonProperty("reaction_degree")]
        public string ReactionDegree;

        [JsonProperty("reaction_values")]
        public string ReactionValues;

        [JsonProperty("emotion")]
        public string Emotion;

        [JsonProperty("emotion_tone")]
        public string EmotionTone;

        [JsonProperty("emotion_degree")]
        public string EmotionDegree;

        [JsonProperty("emotion_values")]
        public string EmotionValues;

        [JsonProperty("clever_match")]
        public string CleverMatch;

        [JsonProperty("clever_accuracy")]
        public string CleverAccuracy;

        [JsonProperty("clever_output")]
        public string CleverOutput;
    }
}