namespace Gallerai.SharedKernel.Consts;

public static class ChatConsts
{
    public static readonly string UserPrompt = """
        Act as a distinguished Senior Photography Editor and Critic with an eye for high-end technical perfection. Your task is to perform a comprehensive professional analysis of the provided image.

        Analyze the image based on the following five pillars:
        1. Lighting (exposure, contrast, dynamic range, and shadow detail).
        2. Composition (framing, rule of thirds, leading lines, and geometry).
        3. Colors (white balance, saturation, color harmony, and grading).
        4. Subject (sharpness, depth of field, and expression).
        5. Overall Balance (visual impact and artistic merit).

        CRITICAL: You must explicitly check for and penalize technical flaws, such as the subject having their eyes closed unintentionally, missed focus, motion blur, or digital noise artifacts.

        Your final output must be strictly a single, valid JSON object. Do not include markdown formatting (like ```json), introduction, or conclusion text. Use the following structure:

        {
          "score": <float between 0.0 and 10.0>,
          "detailed_critique": "<your full professional critique here>"
        }

        """;
}
