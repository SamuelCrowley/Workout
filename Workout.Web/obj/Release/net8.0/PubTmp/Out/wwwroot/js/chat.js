class Chat {
    static initialize(connection) {
        this.connection = connection;
        this.config = {};

        document.getElementById("sendMessageBtn").addEventListener("click", () => {
            Chat.sendMessage();
        });

        this.bindEvents();
        this.startConnection();
    }

    static bindEvents() {
        const messageInput = document.getElementById("messageInput");
        if (messageInput) {
            messageInput.addEventListener("keydown", (event) => {
                if (event.key === "Enter" && event.target.value.trim() !== "") {
                    this.sendMessage();
                }
            });
        }
    }

    static async startConnection() {
        try {
            await this.connection.start();

            this.connection.on("ReceiveMessage", (user, message, color) => {
                const messagesList = document.getElementById("messagesList");
                if (!messagesList) return;

                const msg = document.createElement("li");
                msg.className = "list-group-item";
                msg.innerHTML = `<strong style="color:${color}">${user}:</strong> ${message}`;
                messagesList.appendChild(msg);
                messagesList.scrollTo({
                    top: messagesList.scrollHeight,
                    behavior: 'smooth'
                });
            });
        } catch (err) {
            console.error("SignalR Connection Error:", err.toString());
            setTimeout(() => this.startConnection(), 5000);
        }
    }

    static async sendMessage() {
        const messageInput = document.getElementById("messageInput");
        const message = messageInput.value.trim();
        if (!message) return;

        try {
            await this.connection.invoke("SendMessage", message);
            messageInput.value = "";
            messageInput.focus();
        } catch (err) {
            console.error("Error sending message:", err.toString());
        }
    }
}