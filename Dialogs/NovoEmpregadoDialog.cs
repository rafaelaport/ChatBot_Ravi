using CoreBot.Details;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.BotBuilderSamples.Dialogs;
using System.Threading;
using System.Threading.Tasks;

namespace CoreBot.Dialogs
{
    public class NovoEmpregadoDialog: CancelAndHelpDialog
    {
        private const string solicitacaoNomeCompleto = "Qual o nome completo do empregado?";

        public NovoEmpregadoDialog()
            : base(nameof(NovoEmpregadoDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
           // AddDialog(new DateResolverDialog());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                VerificacaoNomeCompletoStepAsync,
                VerificacaoMatriculaStepAsync,
                //TravelDateStepAsync,
                //ConfirmStepAsync,
                //FinalStepAsync,
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> VerificacaoNomeCompletoStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var novoEmpregadoDetails = (NovoEmpregadoDetails)stepContext.Options;

            if (novoEmpregadoDetails.NomeCompleto == null)
            {
                var promptMessage = MessageFactory.Text(solicitacaoNomeCompleto, solicitacaoNomeCompleto, InputHints.ExpectingInput);
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
            }
            /*else
            {

                //TODO: alterar Yes e No para Sim e Não
                //TODO: fazer o tratamento dependendo da resposta, se for sim, solicitar matricula, se for não pedir o nome completo do funcionário
                var confirmacaoNomeCompleto = $"O nome completo do novo empregado é: {novoEmpregadoDetails.NomeCompleto}?";
                var promptMessage = MessageFactory.Text(confirmacaoNomeCompleto, confirmacaoNomeCompleto, InputHints.ExpectingInput);

               return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
               
            }*/

            return await stepContext.NextAsync(novoEmpregadoDetails.NomeCompleto, cancellationToken);
        }

        private async Task<DialogTurnResult> VerificacaoMatriculaStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var novoEmpregadoDetails = (NovoEmpregadoDetails)stepContext.Options;

            novoEmpregadoDetails.NomeCompleto = (string)stepContext.Result;

            if (novoEmpregadoDetails.Matricula == null)
            {
                var solicitacaoMatricula = $"Qual é a matrícula de {novoEmpregadoDetails.NomeCompleto}?";
                var promptMessage = MessageFactory.Text(solicitacaoMatricula, solicitacaoMatricula, InputHints.ExpectingInput);
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
            }

            return await stepContext.NextAsync(novoEmpregadoDetails.Matricula, cancellationToken);
        }
    }
}
