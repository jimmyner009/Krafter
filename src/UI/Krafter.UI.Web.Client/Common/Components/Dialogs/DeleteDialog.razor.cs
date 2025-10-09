using Krafter.Api.Client;
using Krafter.Api.Client.Models;
using Krafter.UI.Web.Client.Common.Enums;
using Response = Krafter.UI.Web.Client.Common.Models.Response;

namespace Krafter.UI.Web.Client.Common.Components.Dialogs;

public partial class DeleteDialog(
        DialogService dialogService,
        NotificationService notificationService,
        KrafterClient krafterClient


    ) : ComponentBase
    {
        [Parameter] public DeleteRequestInput? DeleteRequestInput { get; set; } = new DeleteRequestInput();

        private bool isBusy = false;

        private async Task Submit(DeleteRequestInput? deleteRequestInput)
        {
            if (deleteRequestInput is { })
            {
                Response result = new Response();

                isBusy = true;
                switch (deleteRequestInput.EntityKind)
                {
                    case (int)EntityKind.None:
                        break;

                    case (int)EntityKind.KrafterRole:
                        var result3 = await krafterClient.Roles.DeletePath.PostAsync(
                            new Api.Client.Models.DeleteRequestInput()
                            {
                                DeleteReason = deleteRequestInput.DeleteReason,
                                Id = deleteRequestInput.Id,



                            });

                        if (result3 is { })
                        {
                            result.IsError = result3 is { IsError: true };
                            if (result3.StatusCode is not null)
                            {
                                result.StatusCode = result3.StatusCode.Value;
                            }
                            result.Message = result3.Message;
                        }
                        break;

                    case (int) EntityKind.KrafterUser:
                        var result1 =await krafterClient.Users.DeletePath.PostAsync(
                            new Api.Client.Models.DeleteRequestInput()
                            {
                                DeleteReason = deleteRequestInput.DeleteReason,
                                Id = deleteRequestInput.Id,
                                


                            });

                        if (result1 is {})
                        {
                            result.IsError= result1 is { IsError: true };
                            if (result1.StatusCode is not null)
                            {
                                result.StatusCode = result1.StatusCode.Value;
                            }
                            result.Message = result1.Message;
                        }
                        break;

                    case (int)EntityKind.Tenant:
                        // result = await tenantService.DeleteAsync(deleteRequestInput);


                        var result2 = await krafterClient.Tenants.DeletePath.PostAsync(
                            new Api.Client.Models.DeleteRequestInput()
                            {
                                DeleteReason = deleteRequestInput.DeleteReason,
                                Id = deleteRequestInput.Id,



                            });

                        if (result2 is { })
                        {
                            result.IsError = result2 is { IsError: true };
                            if (result2.StatusCode is not null)
                            {
                                result.StatusCode = result2.StatusCode.Value;
                            }
                            result.Message = result2.Message;
                        }

                        break;

                    default:
                        isBusy = false;
                        //show error using notification service
                        notificationService.Notify(NotificationSeverity.Error, "Error", "Invalid Entity Kind");
                        return;
                        break;
                }

                isBusy = false;
                StateHasChanged();
                if (!result.IsError)
                {
                    dialogService.Close(true);
                }
            }
        }
    }