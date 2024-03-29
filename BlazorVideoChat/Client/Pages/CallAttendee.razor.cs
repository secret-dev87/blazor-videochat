﻿using BlazorVideoChat.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace BlazorVideoChat.Client.Pages
{
    public class CallAttendeeBase : ComponentBase
    {
        [Inject]
        IJSRuntime _js { get; set; }

        [Inject]
        IHttpClientFactory _httpClientFactory { get; set; }

        private HttpClient _httpClient { get; set; }

        protected ElementReference MyVideo { get; set; }
        protected ElementReference RemoteVideo { get; set; }

        protected string HostEmail { get; set; }
        protected bool RequestCallDisabled { get; set; } = false;
        protected CommunicationModel CommModel { get; set; } = null;

        protected override async Task OnInitializedAsync()
        {
            _httpClient = _httpClientFactory.CreateClient("BlazorVideoChat.ServerAPI.Public");

            await base.OnInitializedAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                CommModel = await _httpClient.GetFromJsonAsync<CommunicationModel>("api/commtoken/");

                if (CommModel is null) throw new ArgumentNullException(nameof(CommModel));

                await _js.InvokeVoidAsync("videoChat.init", CommModel.CommunicationsToken, MyVideo, RemoteVideo);

                StateHasChanged();
            }
        }

        protected async Task RequestCall()
        {
            // Send a post request to create a new CallData entry in the database
            await _httpClient.PostAsJsonAsync($"api/calldata/{Uri.EscapeDataString(HostEmail)}", CommModel.CommunicationUser.Id);
            RequestCallDisabled = true;
            StateHasChanged();
        }
    }
}
