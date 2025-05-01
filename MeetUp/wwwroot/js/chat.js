class Chat {
    static initialize(connection) {
        this.connection = connection;
        this.config = {};

        document.getElementById("messagesList").classList.add("loaded");
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
                if (!messagesList) {
                    return;
                }

                const container = document.querySelector('.messages-container');
                if (!container) {
                    return;
                }
                const attribute = container.attributes[0]?.name;

                const li = document.createElement("li");
                li.className = "list-group-item message-item";
                li.setAttribute(attribute, "");

                const strong = document.createElement("strong");
                strong.style.color = color;
                strong.textContent = `${user}: `; 

                const messageText = document.createTextNode(message);

                li.appendChild(strong);
                li.appendChild(messageText); 

                messagesList.appendChild(li);

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
        if (!message) {
            return;
        }

        try {
            await this.connection.invoke("SendMessage", message);
            messageInput.value = "";
            messageInput.focus();
        } catch (err) {
            console.error("Error sending message:", err.toString());
        }
    }
}