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
           //AddDialog(new DateResolverDialog());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                VerificacaoNomeCompletoStepAsync,
                VerificacaoMatriculaStepAsync,
                VerificacaoNomeSolicitanteStepAsync,
                ConfirmacaoDadosStepAsync,
                SolicitacaoFinalizadaStepAsync
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

        private async Task<DialogTurnResult> VerificacaoNomeSolicitanteStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
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

        private async Task<DialogTurnResult> ConfirmacaoDadosStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var novoEmpregadoDetails = (NovoEmpregadoDetails)stepContext.Options;

            novoEmpregadoDetails.NomeSolicitante = (string)stepContext.Result;

            var confirmacaoDados = $"Os dados abaixo estão corretos? \r\n" +
                $"Nome: { novoEmpregadoDetails.NomeCompleto } \r\n" +
                $"Matrícula: { novoEmpregadoDetails.Matricula } \r\n" +
                $"Solicitante: { novoEmpregadoDetails.NomeSolicitante }";

            var promptMessage = MessageFactory.Text(confirmacaoDados, confirmacaoDados, InputHints.ExpectingInput);

            return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);


        }

        private async Task<DialogTurnResult> SolicitacaoFinalizadaStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if ((bool)stepContext.Result)
            {
                var novoEmpregadoDetails = (NovoEmpregadoDetails)stepContext.Options;

                var solicitacaoFinalizada = $"Sua solicitação foi finalizada com sucesso e o número é 3030.";

                var promptMessage = MessageFactory.Text(solicitacaoFinalizada, solicitacaoFinalizada, InputHints.ExpectingInput);

                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
            }

            return await stepContext.BeginDialogAsync(InitialDialogId);

        }
    }
}
