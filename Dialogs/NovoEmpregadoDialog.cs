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
        private const string solititacaoNomeSolicitante = "Em nome de quem essa solicitação vai ser criada?";

        public NovoEmpregadoDialog()
            : base(nameof(NovoEmpregadoDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
           // AddDialog(new DateResolverDialog());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                VerificacaoNomeCompletoNuloOuNaoStepAsync,
                ConfirmacaoNomeCompletoStepAsync,
                VerificacaoMatriculaStepAsync,
                VerificacaoNomeSolicitante
                
                //TravelDateStepAsync,
                //ConfirmStepAsync,
                //FinalStepAsync,
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> VerificacaoNomeCompletoNuloOuNaoStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var novoEmpregadoDetails = (NovoEmpregadoDetails)stepContext.Options;

            if (novoEmpregadoDetails.NomeCompleto == null)
            {
                var promptMessage = MessageFactory.Text(solicitacaoNomeCompleto, solicitacaoNomeCompleto, InputHints.ExpectingInput);
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
            }
            else
            {

                //TODO: alterar Yes e No para Sim e Não
                var confirmacaoNomeCompleto = $"O nome completo do novo empregado é: {novoEmpregadoDetails.NomeCompleto}?";
                var promptMessage = MessageFactory.Text(confirmacaoNomeCompleto, confirmacaoNomeCompleto, InputHints.ExpectingInput);

                return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
            }
        }

        private async Task<DialogTurnResult> ConfirmacaoNomeCompletoStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var novoEmpregadoDetails = (NovoEmpregadoDetails)stepContext.Options;

            if ((bool)stepContext.Result && novoEmpregadoDetails.NomeCompleto != null)
            {
                return await stepContext.NextAsync(novoEmpregadoDetails.NomeCompleto, cancellationToken);
            }
            else
            {
                var promptMessage = MessageFactory.Text(solicitacaoNomeCompleto, solicitacaoNomeCompleto, InputHints.ExpectingInput);
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
            }
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

        private async Task<DialogTurnResult> VerificacaoNomeSolicitante(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var novoEmpregadoDetails = (NovoEmpregadoDetails)stepContext.Options;

            novoEmpregadoDetails.Matricula = (string)stepContext.Result;

            if (novoEmpregadoDetails.NomeSolicitante == null)
            {
                var promptMessage = MessageFactory.Text(solititacaoNomeSolicitante, solititacaoNomeSolicitante, InputHints.ExpectingInput);
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
            }

            return await stepContext.NextAsync(novoEmpregadoDetails.NomeSolicitante, cancellationToken);
        }
    }
}
