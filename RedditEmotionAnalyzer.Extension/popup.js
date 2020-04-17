document.addEventListener('readystatechange', (event) => {
    if (document.readyState === "complete") {
        chrome.tabs.query({ active: true, lastFocusedWindow: true }, tabs => {
            let url = tabs[0].url;

            // url to call: https://redditemotionanalyzerapp20200415132941.azurewebsites.net/api/RedditThreadAnalyzer_HttpStart?url=
            let apiUri = `https://redditemotionanalyzerapp20200415132941.azurewebsites.net/api/RedditThreadAnalyzer_HttpStart?url=${encodeURIComponent(url)}`;
            let intervalId;
            fetch(apiUri)
                .then(response => response.json())
                .then(data => {
                    return data.statusQueryGetUri;
                })
                .then(data => {
                    intervalId = setInterval(() => {
                        fetch(data)
                            .then(response => response.json())
                            .then(result => {
                                document.getElementById("status").innerText = result.runtimeStatus;
                                if (result.runtimeStatus !== "Running" && result.runtimeStatus !== "Pending") {
                                    document.getElementById("status").hidden = true;
                                    clearInterval(intervalId);
                                }

                                if (result.runtimeStatus === "Completed") {
                                    let output = result.output
                                    let detailedResult = `Positive: ${output.Positive}<br/>Negative: ${output.Negative}<br/>Neutral: ${output.Neutral}<br/>Mixed: ${output.Mixed}`;
                                    document.getElementById('detailedResult').innerHTML = detailedResult;
                                    if (output.Positive > output.Negative && output.Positive > output.Neutral) {
                                        document.getElementById('imageResult').src = 'icons/reddit-128-happy.png';
                                    }

                                    if (output.Negative > output.Positive && output.Negative > output.Neutral) {
                                        document.getElementById('imageResult').src = 'icons/reddit-128-sad.png';
                                    }

                                    if (output.Neutral >= output.Positive && output.Neutral >= output.Negative) {
                                        document.getElementById('imageResult').src = 'icons/reddit-128-neutral.png';
                                    }

                                    if (output.Mixed >= output.Positive && output.Mixed >= output.Negative) {
                                        document.getElementById('imageResult').src = 'icons/reddit-128-neutral.png';
                                    }
                                }
                            });
                    }, 1000);
                });

        });
    }
});
