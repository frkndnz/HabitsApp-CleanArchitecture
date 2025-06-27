using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HabitsApp.Application.Services;
using HabitsApp.Domain.Shared;
using MediatR;

namespace HabitsApp.Application.Gemini;
public sealed class SendGeminiPromptCommand : IRequest<Result<SendGeminiResponse>>
{
    public string Prompt { get; set; } = default!;
}

public class SendGeminiResponse
{
    public string Content { get; set; }=default!;
}


internal sealed class SendGeminiPromptCommandHandler(
    IGeminiService geminiService
    ) : IRequestHandler<SendGeminiPromptCommand, Result<SendGeminiResponse>>
{
    public async Task<Result<SendGeminiResponse>> Handle(SendGeminiPromptCommand request, CancellationToken cancellationToken)
    {
        var result = await geminiService.SendPromptAsync(request.Prompt);

        if(string.IsNullOrWhiteSpace(result))
        {
            return Result<SendGeminiResponse>.Failure("Empty response from Gemini!");
        }

        return Result<SendGeminiResponse>.Success(new SendGeminiResponse { Content = result },"succesfully get gemini response!");
    }
}
