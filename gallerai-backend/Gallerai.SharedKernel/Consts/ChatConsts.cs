namespace Gallerai.SharedKernel.Consts;

public static class ChatConsts
{
    public static readonly string UserPrompt = "Perform a comprehensive visual analysis of the image. The output must include the final aesthetic score (ranging from 0.0 to 1.0, where 1.0 is the highest) and a detailed critique. #IMPORTANT!!# Present your full response as a single, valid JSON object containing the following keys: score, detailed_critique.";
}
