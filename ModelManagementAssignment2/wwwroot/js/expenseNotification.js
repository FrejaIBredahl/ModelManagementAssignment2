var connection = new signalR.HubConnectionBuilder().withUrl("/expenseNotification").build();

connection.on("newExpenseCreated", function (expense) {
    var message = "New expense created: " + expense;
    var li = document.createElement("li");
    li.textContent = message;
    document.getElementById("expenseList").appendChild(li);
});

connection.start().catch(function (err) { return console.error(err.toString()) });