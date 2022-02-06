using Microsoft.AspNetCore.Components;
using Recruit.Client.Extensions;
using Recruit.Shared;
using Recruit.Shared.ViewModels;
using System.Net.Http.Json;

namespace Recruit.Client.Pages
{
    public partial class Applicants
    {
        [Inject] HttpClient? Http { get; set; }

        private List<Applicant>? applicants;
        private List<Applicant>? filteredApplicants;
        private Applicant? selectedApplicant { get; set; }
        private List<Job>? Positions => applicants?.Select(a => a.Job!).DistinctBy(j => j.Id).ToList();

        private int filterJobIdValue = 0;

        private bool ShowApplicantDetails = false;
        private bool ShowDeleteDialog = false;
        private bool ShowCopyDialog = false;
        private bool ShowMoveDialog = false;
        private bool ShowBulkCopyDialog = false;
        private bool ShowBulkMoveDialog = false;
        private bool ShowBulkDeleteDialog = false;

        private string? ResponsiveCss = "";

        protected override async Task OnInitializedAsync()
        {
            applicants = await Http!.GetFromJsonAsync<List<Applicant>>("api/Applicants");
            filteredApplicants = applicants;
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                // Important: The Virtualize component has problem with detecting height when table-responsive is used.
                // So, we must add the table-responsive css after the Virtualize is initialized.
                Task.Run(async () =>
                {
                    await Task.Delay(50);
                    ResponsiveCss = "table-responsive";
                    StateHasChanged();
                });
            }
        }

        public void ShowDetails(Applicant? applicant)
        {
            ShowApplicantDetails = true;
            selectedApplicant = applicant;
        }

        public void HideDetails()
        {
            ShowApplicantDetails = false;
            selectedApplicant = null;
        }

        private void OpenDeleteDialog(Applicant applicant)
        {
            ShowDeleteDialog = true;
            selectedApplicant = applicant;
            StateHasChanged();
        }

        private void OpenCopyDialog(Applicant applicant)
        {
            selectedApplicant = applicant;
            ShowCopyDialog = true;
            StateHasChanged();
        }

        private void OpenMoveDialog(Applicant applicant)
        {
            selectedApplicant = applicant;
            ShowMoveDialog = true;
            StateHasChanged();
        }

        private async Task OnDeleteDialogClose(bool confirmed)
        {
            if (confirmed && selectedApplicant != null)
            {
                var response = await Http!.DeleteAsync($"api/Applicants/{selectedApplicant.Id}");
                if (response.IsSuccessStatusCode)
                {
                    applicants?.Remove(selectedApplicant);
                    ApplyFilters();
                }

                selectedApplicants.Clear();
            }

            ShowDeleteDialog = false;
            StateHasChanged();
        }

        private void HandleStageUpdate(Applicant applicant)
        {
            var applicantToUpdate = applicants?.FirstOrDefault(a => a.Id == applicant.Id);
            if (applicantToUpdate != null)
            {
                applicantToUpdate.Stage = applicant.Stage;
                StateHasChanged();
            }
        }

        private void HandleCancel()
        {
            selectedApplicant = null;
            ShowCopyDialog = false;
            ShowMoveDialog = false;
            ShowDeleteDialog = false;
            ShowBulkCopyDialog = false;
            ShowBulkMoveDialog = false;
            ShowBulkDeleteDialog = false;
        }

        private void HandleCopy(Applicant applicant)
        {
            int index = applicants?.FindIndex(a => a.Id == selectedApplicant?.Id) ?? 0;
            applicants?.Insert(index + 1, applicant);
            ShowCopyDialog = false;
            selectedApplicants.Clear();
            StateHasChanged();
        }

        private void HandleMove(Applicant applicant)
        {
            applicants?.Replace(selectedApplicant!, applicant);
            ShowMoveDialog = false;
            selectedApplicants.Clear();
            ApplyFilters();
            StateHasChanged();
        }

        private void FitlerByPosition(ChangeEventArgs args)
        {
            if (int.TryParse(args.Value?.ToString(), out int value))
            {
                filterJobIdValue = value;
                ApplyFilters();
            }
        }

        private void ApplyFilters()
        {
            // Reset position filter if theres no position in list after the changes
            if (!Positions!.Any(j => j.Id == filterJobIdValue))
                filterJobIdValue = 0;

            // Apply filters
            filteredApplicants = filterJobIdValue > 0 ?
                    applicants?.Where(a => a.Job?.Id == filterJobIdValue).ToList() : applicants;

            // Refresh selected items
            selectedApplicants = selectedApplicants.Where(a => filteredApplicants!.Contains(a)).ToList();
        }

        #region Bulk Actions
        private List<Applicant> selectedApplicants = new();
        private bool IsBulkActionsDisabled => selectedApplicants.Count == 0;
        private bool IsSelectAllChecked => selectedApplicants.Count == filteredApplicants?.Count && selectedApplicants.Count > 0;
        
        private void ToggleSelect(bool isChecked, Applicant applicant)
        {
            if (isChecked)
                selectedApplicants.Add(applicant);
            else
                selectedApplicants.Remove(applicant);
        }

        private void SelectAll()
        {
            if (selectedApplicants.Count == filteredApplicants?.Count)
                selectedApplicants.Clear();
            else
                selectedApplicants = new List<Applicant>(filteredApplicants!);
        }

        private async Task OnBulkDeleteDialogClose(bool confirmed)
        {
            if (confirmed)
            {
                var viewModel = new BulkActionViewModel() { Items = selectedApplicants.Select(a => a.Id).ToList() };
                var response = await Http!.PostAsJsonAsync($"api/BulkActions/DeleteApplicants", viewModel);
                if (response.IsSuccessStatusCode)
                {
                    var deletedApplicants = await response.Content.ReadFromJsonAsync<List<int>>();
                    if (deletedApplicants?.Count > 0)
                    {
                        applicants = applicants?.Where(a => !deletedApplicants.Contains(a.Id)).ToList();
                        ApplyFilters();
                    }
                }

                selectedApplicants.Clear();
            }

            ShowBulkDeleteDialog = false;
            StateHasChanged();
        }

        private void HandleBulkCopy(List<Applicant> updatedApplicants)
        {
            ShowBulkCopyDialog = false;
            selectedApplicants.Clear();

            if (updatedApplicants?.Count > 0)
            {
                updatedApplicants = updatedApplicants.OrderByDescending(a => a.ApplyDate).ToList();
                applicants?.InsertRange(0, updatedApplicants);
            }

            StateHasChanged();
        }

        private void HandleBulkMove(List<Applicant> updatedApplicants)
        {
            ShowBulkMoveDialog = false;
            selectedApplicants.Clear();

            foreach (var applicant in updatedApplicants)
            {
                var applicantToUpdate = applicants?.FirstOrDefault(a => a.Id == applicant.Id);
                if (applicantToUpdate != null)
                    applicants.Replace(applicantToUpdate, applicant);

                ApplyFilters();
            }

            StateHasChanged();
        }

        #endregion
    }
}
