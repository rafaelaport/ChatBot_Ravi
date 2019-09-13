using CoreBot.Details;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.BotBuilderSamples.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CoreBot.CognitiveModels
{
    public class NovoEmpregadoDialog : CancelAndHelpDialog
    {
        private const string DestinationStepMsgText = "Deseja cadastrar um novo empregado?";

        public NovoEmpregadoDialog()
            : base(nameof(NovoEmpregadoDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new DateResolverDialog());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                DestinationStepAsync,
                //OriginStepAsync,
                //TravelDateStepAsync,
                //ConfirmStepAsync,
                //FinalStepAsync,
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> DestinationStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var novoEmpregadoDetails = (NovoEmpregadoDetails)stepContext.Options;

            if (novoEmpregadoDetails.NomeCompleto == null)
            {
                var promptMessage = MessageFactory.Text(DestinationStepMsgText, DestinationStepMsgText, InputHints.ExpectingInput);
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
            }

            return await stepContext.NextAsync(novoEmpregadoDetails.NomeCompleto, cancellationToken);
        }
    }
}
