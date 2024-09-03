// Add event listener to the button that triggers the analysis.
document.getElementById('analyzeBtn').addEventListener('click', function() {
    // Get the username from the input field.
    const username = document.getElementById('instagramUsername').value;
    if (username) {
        // Fetch analysis results if username is not empty.
        fetchAnalysisResults(username);
    } else {
        // Alert the user if the username input is empty.
        alert('Please enter an Instagram username.');
    }
});

function showSpinner() {
    document.getElementById('spinner').classList.remove('d-none');
}

function hideSpinner() {
    document.getElementById('spinner').classList.add('d-none');
}

function fetchAnalysisResults(username) {
    showSpinner();  // Show spinner before making the request.

    postAnalytics(username)
        .then(analysisResults => {
            // Assuming analysisResults comes in the same structure as your mock data.
            updateUI(analysisResults);
            hideSpinner();  // Hide spinner after processing the response.
            document.getElementById('resultsContainer').classList.remove('d-none'); // Make the results container visible.
        })
        .catch(error => {
            console.error('Error fetching analysis results:', error);
            hideSpinner();
            alert('Failed to fetch analysis results.');
        });
}

function updateUI(analysisResults) {
    // Update the UI with the fetched data.
    console.log(analysisResults);
    document.getElementById('accountNameVal').textContent = analysisResults.accountName;
    document.getElementById('businessDescriptionVal').textContent = analysisResults.businessDescription;
    document.getElementById('businessAreaVal').textContent = analysisResults.businessArea;
    document.getElementById('regionVal').textContent = analysisResults.region;
    document.getElementById('targetNichesVal').textContent = analysisResults.targetNiches;
    document.getElementById('hashtagIdeasVal').textContent = analysisResults.hashtagIdeas;
    document.getElementById('improvementsSuggestionsVal').textContent = analysisResults.improvementsSuggestions;
    document.getElementById('analyticsVal').textContent = analysisResults.analitics;
    document.getElementById('potentialPartnersVal').textContent = analysisResults.potentialPartners;
    document.getElementById('trendsContentVal').textContent = analysisResults.trendsContent;
    document.getElementById('topicsContentVal').textContent = analysisResults.topicsContent;

    updateGauge(analysisResults.brandValue);
}

function updateGauge(percentage) {
    // Calculate the rotation based on percentage
    const rotation = percentage * 1.8; // Maps 0-100% to -90deg to 90deg

    // Select the gauge element
    const gauge = document.querySelector('.gauge');
    console.log(rotation);
    // Update the custom properties and text content
    gauge.style.setProperty('--rotation', `${rotation}deg`);

    // Update the percentage value displayed
    document.getElementById('brandRate').textContent = `${percentage}%`;
}

function postAnalytics(userName) {
    const encodedUserName = encodeURIComponent(userName);
    const url = `https://localhost:5220/analitics?userName=${encodedUserName}`;

    return fetch(url, {
        method: 'GET',
        // mode: 'cors',e
        // headers: {
        //     'Content-Type': 'application/json'
        //     // Removed 'Access-Control-Allow-Origin' header because it should be set on the server, not on the client request.
        // }
    })
    .then(response => {
        if (!response.ok) {
            throw new Error('Network response was not ok');
        }
        return response.json();  // Convert the response data to JSON
    });
}
