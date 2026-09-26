using System.Text;
using System.Text.Json;

public class GeminiService
{
    private readonly string _apiKey = "AQ.Ab8RN6LkBLfJTjeQs9Xn1g_997WjtM-klPeMLSIzQ-KKn188wQ";
    private readonly HttpClient _httpClient;

    public GeminiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> GenerateTourDataAsync(string userPrompt)
    {
        // Binago natin ang URL patungo sa Google Cloud / Vertex AI endpoint format na tumatanggap sa AQ. keys
        var url = "https://aiplatform.googleapis.com/v1/publishers/google/models/gemini-1.5-flash:generateContent";

        string systemPrompt = @"
Ikaw ang AI Tour Assistant para sa 'Meesuk Travel and Tours'. 
Basahin ang hiling ng user at magbalik ng STRICT JSON format lamang para mapunan ang Tour form. 
Huwag maglagay ng anumang extra text o backticks. Sundin ang eksaktong keys na ito:
{
  ""TourName"": ""Pangalan ng Tour Package"",
  ""Destination"": ""Lokasyon o Bansa"",
  ""DepartureDate"": ""YYYY-MM-DD"",
  ""ReturnDate"": ""YYYY-MM-DD"",
  ""Description"": ""Buong deskripsyon ng tour"",
  ""BasePrice"": 0.00,
  ""OperationalCost"": 0.00,
  ""TotalCapacity"": 15,
  ""MinParticipants"": 5,
  ""Deadline"": ""YYYY-MM-DD"",
  ""VehicleAssignment"": ""Hal. Van o Bus"",
  ""AccommodationAssignment"": ""Hal. Hotel Name"",
  ""ItineraryHighlights"": ""Mga highlights na naka-comma separated""
}
";

        var fullPayload = new
        {
            contents = new[]
            {
                new { parts = new[] { new { text = $"{systemPrompt}\n\nUser Request: {userPrompt}" } } }
            }
        };

        var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
        
        // Ang mga AQ. keys ay gumagana bilang Bearer token o x-goog-api-key sa Vertex endpoint na ito
        requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);
        requestMessage.Content = new StringContent(JsonSerializer.Serialize(fullPayload), Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(requestMessage);
        var responseString = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Gemini API Error ({response.StatusCode}): {responseString}");
        }

        using var doc = JsonDocument.Parse(responseString);
        
        var rawText = doc.RootElement.GetProperty("candidates")[0]
                                   .GetProperty("content")
                                   .GetProperty("parts")[0]
                                   .GetProperty("text").GetString();

        return rawText.Replace("```json", "").Replace("```", "").Trim();
    }
}
