class GymTracker {
    static config = {
        difficultyOptions: [],
        antiforgeryToken: ''
    };

    static state = {
        currentSessionId: null,
        workoutName: null, // Added to store workout name
        currentExerciseId: null,
        currentExerciseName: null, // Added to store exercise name
        currentSetId: null,
        workoutStartTime: null,
        timerInterval: null,
        restTimerEndTime: null,
        restTimerInterval: null, // Added rest timer interval to state
        exercises: [],
        currentReps: [],
        editingSet: false,
        restTimerRemaining: 90 // Initialize with default rest duration
    };

    static initialize(config) {
        this.config = config;
        this.bindEvents();
        this.checkGymUserExists();
        // checkActiveWorkout is called within checkGymUserExists now
    }

    static bindEvents() {
        document.getElementById("startWorkoutBtn")?.addEventListener("click", () => this.startWorkout());
        document.getElementById("createGymUserBtn")?.addEventListener("click", () => this.createGymUser());
        document.getElementById("endWorkoutBtn")?.addEventListener("click", () => this.endWorkout());
        document.getElementById("addExerciseBtn")?.addEventListener("click", () => this.addExercise());
        document.getElementById("addSetBtn")?.addEventListener("click", () => this.addSet());
        document.getElementById("backToWorkoutBtn")?.addEventListener("click", () => this.backToWorkout());
        document.getElementById("backToExerciseBtn")?.addEventListener("click", () => this.backToExercise());
        document.getElementById("addRepsBtn")?.addEventListener("click", () => this.addReps());
        document.getElementById("clearRepsBtn")?.addEventListener("click", () => this.clearReps());
        document.getElementById("saveSetBtn")?.addEventListener("click", () => this.saveSet());
        document.getElementById("startRestTimerBtn")?.addEventListener("click", () => this.startRestTimer());
        document.getElementById("stopRestTimerBtn")?.addEventListener("click", () => this.stopRestTimer());

        // Use event delegation for dynamically added reps list items
        document.getElementById("repsList")?.addEventListener("click", (e) => {
            const target = e.target.closest('.badge, .btn'); // Get the closest interactive element
            if (!target) return;

            const repItem = target.closest('.list-group-item.rep-item');
            if (!repItem) return;

            // Extract rep ID - assuming it's stored somewhere or can be derived
            // Based on renderRepsList, repId is passed directly in the onclick handler,
            // so delegation needs a different approach or keep onclick.
            // Keeping onclick for simplicity with current structure, but delegation is more performant for many items.
            // We'll rely on the onclick attributes present in renderRepsList for now.
        });


        document.getElementById("restTimerDuration")?.addEventListener("change", (e) => {
            this.state.restTimerRemaining = parseInt(e.target.value);
            this.stopRestTimer(); // Stop current timer
            // Update display immediately
            const minutes = Math.floor(this.state.restTimerRemaining / 60).toString().padStart(2, '0');
            const seconds = (this.state.restTimerRemaining % 60).toString().padStart(2, '0');
            document.getElementById("restTimerDisplay").textContent = `Rest: ${minutes}:${seconds}`;
        });

        //document.getElementById('viewHistoryBtn')?.addEventListener('click', async () => {
        //    const historyModalEl = document.getElementById('historyModal');

        //    if (historyModalEl.dataset.processing === 'true') {
        //        return;
        //    }

        //    historyModalEl.dataset.processing = 'true';

        //    let modal = bootstrap.Modal.getInstance(historyModalEl);
        //    if (!modal) {
        //        modal = new bootstrap.Modal(historyModalEl, {
        //            keyboard: true,
        //            backdrop: true
        //        });
        //    }

        //    // Removed cleanup function logic here, Bootstrap modal handles body classes/padding

        //    historyModalEl.addEventListener('hidden.bs.modal', () => {
        //        document.getElementById('historyContent').style.display = 'none';
        //        document.getElementById('historyLoading').style.display = 'block';
        //        document.getElementById('workoutSessionsList').innerHTML = ''; // Clear session list
        //        document.getElementById('sessionExercises').innerHTML = ''; // Clear details
        //        document.getElementById('historyModal').dataset.processing = 'false';
        //    }, { once: true });


        //    try {
        //        document.getElementById('historyContent').style.display = 'none'; // Hide content, show loading
        //        document.getElementById('historyLoading').style.display = 'block';
        //        modal.show();
        //        await this.loadWorkoutHistory();
        //    } catch (error) {
        //        document.getElementById('historyLoading').style.display = 'none';
        //        document.getElementById('historyContent').style.display = 'block';
        //        document.getElementById('workoutSessionsList').innerHTML = '<li class="list-group-item text-danger">Error loading history.</li>';
        //        historyModalEl.dataset.processing = 'false';
        //        console.error('Error showing history modal:', error);
        //    }
        //});


        // Initialize tooltips if needed (assuming you added data-bs-toggle="tooltip")
        const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
        tooltipTriggerList.map(tooltipTriggerEl => new bootstrap.Tooltip(tooltipTriggerEl));
    }

    // Helper to switch between main views
    static showView(viewIdToShow) {
        const views = ["initialView", "activeWorkoutView", "exerciseDetailView", "setDetailView"];

        views.forEach(viewId => {
            const viewElement = document.getElementById(viewId);
            if (viewElement) {
                if (viewId === viewIdToShow) {
                    // Remove the hiding class to show the target view
                    viewElement.classList.remove('force-hide');
                    // The .d-flex class already on the element will now correctly apply display: flex;
                    // You can optionally clear the inline style if needed, but removing force-hide should suffice
                    // viewElement.style.display = "";
                } else {
                    // Add the hiding class to hide other views
                    viewElement.classList.add('force-hide');
                    // You can optionally clear the inline style, but adding force-hide should suffice
                    // viewElement.style.display = "";
                }
            } else {
                console.error(`View element with ID "${viewId}" not found.`);
            }
        });

        // Manage the context title visibility using force-hide as well
        const contextTitleElement = document.getElementById('workoutContextTitle');
        if (contextTitleElement) {
            if (viewIdToShow === 'initialView') {
                contextTitleElement.classList.add('force-hide');
            } else {
                contextTitleElement.classList.remove('force-hide');
                // Assuming contextTitleElement is block or flex by default CSS when force-hide is removed
                // If not, you might need contextTitleElement.style.display = 'block'; here
            }
        } else {
            console.error("Workout context title element not found.");
        }
    }

    // Helper to update the persistent context title
    static updateContextTitle(text) {
        document.getElementById('currentWorkoutContext').textContent = text;
    }


    static async callApi(endpoint, method = 'GET', body = null) {
        const headers = {
            'Accept': 'application/json',
            'RequestVerificationToken': this.config.antiforgeryToken
        };

        if (method !== 'GET' && body !== null) { // Only set Content-Type if body is present
            headers['Content-Type'] = 'application/json';
        }

        try {
            const response = await fetch(`/api/GymApi/${endpoint}`, {
                method,
                credentials: 'include',
                headers,
                body: body ? JSON.stringify(body) : null
            });

            if (!response.ok) {
                // Attempt to read error details from response body
                const errorData = await response.json().catch(() => ({ message: `HTTP error! status: ${response.status}` }));
                throw new Error(errorData.message || `HTTP error! status: ${response.status}`);
            }

            // Handle cases where response might not have a body (e.g., DELETE success)
            if (response.status === 204) { // No Content
                return { success: true }; // Assume success for 204
            }

            return await response.json();

        } catch (error) {
            console.error(`API call failed for ${endpoint}:`, error);
            throw error; // Re-throw to be handled by the calling function
        }
    }

    static async checkGymUserExists() {
        try {
            const existsResponse = await this.callApi('exists');

            document.getElementById("createGymUserBtn").style.display = existsResponse.exists ? "none" : "inline-block";
            document.getElementById("workoutNameContainer").style.display = existsResponse.exists ? "block" : "none";
            document.getElementById("startWorkoutBtn").style.display = "none"; // Hide start button by default

            if (existsResponse.exists) {
                // If user exists, check for active workout
                await this.checkActiveWorkout();
                // If checkActiveWorkout didn't show the active view, show the initial view
                if (!this.state.currentSessionId) {
                    this.showView('initialView');
                    document.getElementById("startWorkoutBtn").style.display = "inline-block"; // Show start if no active workout
                }
            } else {
                // If user doesn't exist, show initial view with create button
                this.showView('initialView');
            }


        } catch (error) {
            alert("Error checking gym user status: " + error.message);
            // Fallback to showing initial view even on error
            this.showView('initialView');
            document.getElementById("createGymUserBtn").style.display = "inline-block";
            document.getElementById("workoutNameContainer").style.display = "none";
        }
    }

    static async checkActiveWorkout() {
        try {
            const response = await this.callApi('activeWorkout');

            if (response.success && response.hasActiveWorkout) {
                const workout = response.workout;
                this.state.currentSessionId = workout.id;
                this.state.workoutName = workout.name; // Store workout name
                this.state.workoutStartTime = new Date(workout.startTime).toISOString();

                // Map exercises and sets, ensuring sets have reps array even if empty
                this.state.exercises = workout.exercises.map(ex => ({
                    id: ex.id,
                    name: ex.name,
                    sets: (ex.sets || []).map(set => ({ // Ensure sets is an array
                        id: set.id,
                        reps: (set.reps || []).map(rep => ({ // Ensure reps is an array
                            id: rep.id,
                            weight: rep.weight,
                            difficulty: rep.difficulty,
                            order: rep.order
                        }))
                    }))
                }));


                this.showView('activeWorkoutView'); // Use helper to show view
                this.updateContextTitle('Workout > ' + this.state.workoutName); // Update title
                document.getElementById("workoutNameDisplay").textContent = this.state.workoutName; // Update workout name display in header
                document.getElementById("restTimerContainer").style.display = "block";

                this.startTimer();
                this.renderExercisesList();
                this.updateExerciseCount();
            }
            // If no active workout, checkGymUserExists will handle showing initial view
            return response;
        } catch (error) {
            // Error checking active workout - might mean no active workout, or a real error
            // Let checkGymUserExists handle the view based on user existence
            console.error("Error checking active workout:", error);
            // Optionally show an alert, but avoid double alerts if checkGymUserExists also fails
            // alert("Error checking active workout: " + error.message);
            return { success: false, hasActiveWorkout: false }; // Indicate no active workout on error
        }
    }

    static async createGymUser() {
        try {
            await this.callApi('createGymUser', 'POST');

            alert("Gym user account created!");

            // Update view and buttons after creation
            document.getElementById("createGymUserBtn").style.display = "none";
            document.getElementById("workoutNameContainer").style.display = "block";
            document.getElementById("startWorkoutBtn").style.display = "inline-block";
            this.showView('initialView'); // Ensure initial view is shown after creating user

        } catch (error) {
            alert("Error creating gym user: " + error.message);
        }
    }

    static async startWorkout() {
        const workoutNameInput = document.getElementById("workoutNameInput");
        const workoutName = workoutNameInput.value.trim();

        if (!workoutName) {
            alert("Please enter a workout name");
            workoutNameInput.focus();
            return;
        }

        try {
            const response = await this.callApi('startWorkout', 'POST', workoutName);
            if (!response.success) {
                throw new Error(response.message);
            }

            this.resetWorkoutData(); // Reset state and UI elements
            this.state.currentSessionId = response.sessionId;
            this.state.workoutName = workoutName; // Store workout name
            this.state.workoutStartTime = new Date().toISOString().replace('Z', ''); // Store start time

            this.startTimer(); // Start workout timer

            this.showView('activeWorkoutView'); // Use helper to show view
            this.updateContextTitle('Workout > ' + this.state.workoutName); // Update title

            document.getElementById("workoutNameDisplay").textContent = this.state.workoutName; // Update workout name display in header
            document.getElementById("restTimerContainer").style.display = "block"; // Show rest timer container
            this.state.restTimerRemaining = parseInt(document.getElementById("restTimerDuration").value); // Reset rest timer duration display


        } catch (error) {
            alert("Error starting workout: " + error.message); // Use error.message for consistency
        }
    }

    static resetWorkoutData() {
        this.state.exercises = [];
        this.state.currentReps = [];
        this.state.currentExerciseId = null;
        this.state.currentExerciseName = null; // Clear exercise name
        this.state.currentSetId = null;
        this.state.currentSessionId = null; // Clear session ID
        this.state.workoutName = null; // Clear workout name
        this.state.workoutStartTime = null; // Clear start time

        this.updateExerciseCount(); // Reset counts to 0

        // Clear UI lists
        document.getElementById("exercisesList").innerHTML = "";
        document.getElementById("setsTableBody").innerHTML = "";
        document.getElementById("repsList").innerHTML = "";

        // Reset input fields
        document.getElementById("exerciseNameInput").value = "";
        document.getElementById("workoutNameInput").value = ""; // Clear workout name input
        document.getElementById("setWeightInput").value = "20";
        document.getElementById("repCountInput").value = "6"; // Reset rep count select
        document.getElementById("defaultDifficultyInput").value = "Moderate"; // Reset difficulty select
    }


    static async endWorkout() {
        if (!this.state.currentSessionId) {
            alert("No active workout to end.");
            return;
        }

        if (!confirm("Are you sure you want to end this workout?")) {
            return; // User cancelled
        }

        try {
            // API call to end the workout
            const response = await this.callApi(`endWorkout/${this.state.currentSessionId}`, 'POST');
            if (!response.success) {
                throw new Error(response.message);
            }

            // Stop timers and reset state
            this.stopTimer();
            this.stopRestTimer();
            this.resetWorkoutData(); // Clear all workout data and UI

            // Switch back to the initial view
            this.showView('initialView');
            // The helper hides the context title for the initial view

            // Ensure correct buttons are shown/hidden on initial view
            document.getElementById("startWorkoutBtn").style.display = "inline-block";
            document.getElementById("restTimerContainer").style.display = "none"; // Hide rest timer container

        } catch (error) {
            alert("Error ending workout: " + error.message);
        }
    }

    static startTimer() {
        this.stopTimer(); // Clear any existing timer

        const updateTimer = () => {
            if (!this.state.workoutStartTime) return; // Stop if no start time

            const startTime = new Date(this.state.workoutStartTime);
            const now = new Date();
            const diffMs = now.getTime() - startTime.getTime();

            const hours = Math.floor(diffMs / (1000 * 60 * 60));
            const minutes = Math.floor((diffMs % (1000 * 60 * 60)) / (1000 * 60));
            const seconds = Math.floor((diffMs % (1000 * 60)) / 1000);

            document.getElementById("workoutTimer").textContent =
                `${String(hours).padStart(2, '0')}:` +
                `${String(minutes).padStart(2, '0')}:` +
                `${String(seconds).padStart(2, '0')}`;
        };

        updateTimer(); // Update immediately
        this.state.timerInterval = setInterval(updateTimer, 1000); // Update every second
    }

    static stopTimer() {
        if (this.state.timerInterval) {
            clearInterval(this.state.timerInterval);
            this.state.timerInterval = null;
        }
    }

    static startRestTimer() {
        this.stopRestTimer(); // Clear any existing timer

        const duration = parseInt(document.getElementById("restTimerDuration").value);
        if (isNaN(duration) || duration <= 0) {
            alert("Please select a valid rest duration.");
            return;
        }

        const endTime = Date.now() + (duration * 1000);
        this.state.restTimerEndTime = endTime;

        this.updateRestTimerDisplay(); // Update immediately
        this.state.restTimerInterval = setInterval(() => this.updateRestTimerDisplay(), 1000);
    }

    static updateRestTimerDisplay() {
        if (!this.state.restTimerEndTime) {
            document.getElementById("restTimerDisplay").textContent = "Rest: 00:00"; // Reset display if no end time
            return;
        }

        const now = Date.now();
        const remaining = Math.max(0, this.state.restTimerEndTime - now);

        const minutes = Math.floor(remaining / 60000);
        const seconds = Math.floor((remaining % 60000) / 1000);

        document.getElementById("restTimerDisplay").textContent =
            `Rest: ${String(minutes).padStart(2, '0')}:${String(seconds).padStart(2, '0')}`;

        // Check if timer has finished
        if (remaining <= 0) {
            this.stopRestTimer();
            // Optional: Add an alert or visual cue that rest is over
            // alert("Rest time is over!");
        }
    }

    static stopRestTimer() {
        if (this.state.restTimerInterval) {
            clearInterval(this.state.restTimerInterval);
            this.state.restTimerInterval = null;
        }
        this.state.restTimerEndTime = null; // Clear end time
        this.state.restTimerRemaining = parseInt(document.getElementById("restTimerDuration")?.value || 90); // Reset remaining display to selected duration
        const minutes = Math.floor(this.state.restTimerRemaining / 60).toString().padStart(2, '0');
        const seconds = (this.state.restTimerRemaining % 60).toString().padStart(2, '0');
        document.getElementById("restTimerDisplay").textContent = `Rest: ${minutes}:${seconds}`;
    }


    static async addExercise() {
        if (!this.state.currentSessionId) {
            alert("Start a workout first.");
            return;
        }

        const exerciseNameInput = document.getElementById("exerciseNameInput");
        const exerciseName = exerciseNameInput.value.trim();
        if (!exerciseName) {
            alert("Please enter an exercise name");
            exerciseNameInput.focus();
            return;
        }

        try {
            const response = await this.callApi(`addExercise/${this.state.currentSessionId}`, 'POST', exerciseName);
            if (!response.success) {
                throw new Error(response.message);
            }

            // Add the new exercise to the state
            this.state.exercises.push({
                id: response.exerciseId,
                name: exerciseName,
                sets: [] // New exercises start with no sets
            });

            // Update UI elements
            this.updateExerciseCount();
            this.renderExercisesList();
            exerciseNameInput.value = ""; // Clear the input field

        } catch (error) {
            alert("Error adding exercise: " + error.message);
        }
    }

    static async deleteExercise(exerciseId) {
        if (!this.state.currentSessionId) return; // Only allow deletion during an active workout

        if (!confirm("Delete this exercise and all its sets? This cannot be undone.")) {
            return; // User cancelled
        }

        try {
            const response = await this.callApi(`deleteExercise/${exerciseId}`, 'DELETE');
            if (!response.success) {
                throw new Error(response.message);
            }

            // Remove the exercise from the state
            this.state.exercises = this.state.exercises.filter(ex => ex.id !== exerciseId);

            // Update UI elements
            this.updateExerciseCount();
            this.renderExercisesList();

            // If the deleted exercise was the one being viewed, go back to the workout view
            if (this.state.currentExerciseId === exerciseId) {
                this.backToWorkout();
            }

        } catch (error) {
            alert("Error deleting exercise: " + error.message);
        }
    }

    static viewExerciseDetails(exerciseId) {
        if (!this.state.currentSessionId) return; // Ensure active workout

        const exercise = this.state.exercises.find(ex => ex.id === exerciseId);
        if (!exercise) {
            console.error(`Exercise with ID ${exerciseId} not found in state.`);
            return;
        }

        // Store current exercise details in state
        this.state.currentExerciseId = exerciseId;
        this.state.currentExerciseName = exercise.name;

        // Use helper to switch view
        this.showView('exerciseDetailView');
        this.updateContextTitle(`Workout > ${this.state.workoutName} > ${this.state.currentExerciseName}`); // Update title

        // Update UI elements for exercise detail view
        document.getElementById("exerciseDetailTitle").textContent = this.state.currentExerciseName;

        this.renderSetsList(); // Render the sets for this exercise
    }

    static addSet() {
        if (!this.state.currentExerciseId) {
            alert("Select an exercise first.");
            return;
        }
        // Reset state for adding a new set
        this.state.currentReps = [];
        this.state.editingSet = false;
        this.state.currentSetId = null; // Ensure no set ID is set when adding

        // Use helper to switch view
        this.showView('setDetailView');
        // Update title to show exercise context for adding set
        this.updateContextTitle(`Workout > ${this.state.workoutName} > ${this.state.currentExerciseName} > Add Set`);


        // Reset UI elements for set detail view
        document.getElementById("setDetailTitle").textContent = `Add Set for ${this.state.currentExerciseName}`; // More specific title
        document.getElementById("setWeightInput").value = "20";
        document.getElementById("repCountInput").value = "6"; // Reset rep count select
        document.getElementById("defaultDifficultyInput").value = "Moderate"; // Reset difficulty select
        document.getElementById("repsList").innerHTML = '<div class="text-muted mb-2">No reps added yet</div>'; // Clear reps list UI


        this.renderRepsList(); // Render the empty reps list
    }

    static addReps() {
        const repCountSelect = document.getElementById("repCountInput");
        const defaultDifficultySelect = document.getElementById("defaultDifficultyInput");
        const setWeightInput = document.getElementById("setWeightInput");

        const repCount = parseInt(repCountSelect.value);
        const defaultDifficulty = defaultDifficultySelect.value;
        const defaultWeight = parseFloat(setWeightInput.value);

        if (isNaN(repCount) || repCount <= 0) {
            alert("Please select a valid number of reps.");
            repCountSelect.focus();
            return;
        }
        if (isNaN(defaultWeight)) {
            alert("Please enter a valid default weight.");
            setWeightInput.focus();
            return;
        }


        // Add reps to the currentReps state array
        for (let i = 0; i < repCount; i++) {
            this.state.currentReps.push({
                // Use a temporary unique ID for client-side tracking before saving
                id: 'temp-rep-' + Date.now() + '-' + Math.random().toString(36).substr(2, 9),
                difficulty: defaultDifficulty,
                weight: defaultWeight,
                order: this.state.currentReps.length + 1 // Assign order based on current list length
            });
        }

        this.renderRepsList(); // Update the reps list UI
    }

    static clearReps() {
        if (confirm("Clear all reps for this set?")) {
            this.state.currentRps = []; // Clear the state array
            this.renderRepsList(); // Update the UI (will show "No reps added yet")
        }
    }

    static updateRepDifficulty(repId) {
        const rep = this.state.currentReps.find(r => r.id === repId);
        if (!rep) {
            console.error(`Rep with ID ${repId} not found in currentReps state.`);
            return;
        }

        const difficultyOptions = Object.keys(this.config.difficultyOptions); // Get keys (string names)
        const currentIndex = difficultyOptions.indexOf(rep.difficulty);
        const nextIndex = (currentIndex + 1) % difficultyOptions.length;
        rep.difficulty = difficultyOptions[nextIndex]; // Update difficulty string

        this.renderRepsList(); // Re-render the reps list to show the change
    }

    static updateRepWeight(repId) {
        const rep = this.state.currentReps.find(r => r.id === repId);
        if (!rep) {
            console.error(`Rep with ID ${repId} not found in currentReps state.`);
            return;
        }

        // Use the rep's current weight or a default if not set
        const currentWeight = rep.weight > 0 ? rep.weight.toString() : (parseFloat(document.getElementById("setWeightInput")?.value) || "0");

        const newWeightStr = prompt("Enter new weight (kg):", currentWeight);

        // Check if prompt was cancelled or input is empty
        if (newWeightStr === null || newWeightStr.trim() === "") {
            return; // User cancelled or entered nothing
        }

        const newWeight = parseFloat(newWeightStr);

        if (!isNaN(newWeight) && newWeight >= 0) { // Validate input is a non-negative number
            rep.weight = newWeight; // Update weight
            this.renderRepsList(); // Re-render list
        } else {
            alert("Invalid weight entered. Please enter a non-negative number.");
        }
    }


    static deleteRep(repId) {
        // Filter out the rep to delete
        const initialCount = this.state.currentReps.length;
        this.state.currentReps = this.state.currentReps.filter(rep => rep.id !== repId);

        // If a rep was actually removed, re-order the remaining reps
        if (this.state.currentReps.length < initialCount) {
            this.state.currentReps.forEach((rep, index) => rep.order = index + 1);
            this.renderRpsList(); // Re-render the list
        } else {
            console.warn(`Attempted to delete rep with ID ${repId} but it was not found.`);
        }
    }


    static async saveSet() {
        if (!this.state.currentExerciseId) {
            alert("An exercise must be selected to save a set.");
            return;
        }

        const setWeightInput = document.getElementById("setWeightInput");
        const defaultWeight = parseFloat(setWeightInput.value);

        if (isNaN(defaultWeight) || defaultWeight < 0) {
            alert("Please enter a valid non-negative weight.");
            setWeightInput.focus();
            return;
        }

        // Ensure all reps have a valid weight (use the default if not set)
        this.state.currentReps.forEach(rep => {
            if (rep.weight === undefined || rep.weight === null || isNaN(rep.weight) || rep.weight < 0) {
                rep.weight = defaultWeight; // Assign default weight if rep weight is invalid/missing
            }
        });


        if (this.state.currentReps.length === 0) {
            alert("Add at least one rep to save the set.");
            return;
        }

        // Prepare reps data for the API call
        const repsToSave = this.state.currentReps.map((rep, index) => ({
            Weight: rep.weight,
            // Ensure difficulty is a string matching the enum names expected by the backend
            Difficulty: typeof rep.difficulty === 'string' ? rep.difficulty : 'Unknown', // Default to 'Unknown' or another fallback if type is wrong
            Order: index + 1 // Ensure order is correct based on client-side list
        }));


        try {
            let response;
            let exerciseIndex = this.state.exercises.findIndex(ex => ex.id === this.state.currentExerciseId);
            if (exerciseIndex === -1) {
                throw new Error("Current exercise not found in state.");
            }
            let currentExercise = this.state.exercises[exerciseIndex];

            if (this.state.editingSet) {
                // Call API to update the existing set
                if (!this.state.currentSetId) {
                    throw new Error("No set ID available for editing.");
                }
                response = await this.callApi(`editSet/${this.state.currentSetId}`, 'PUT', repsToSave); // Use PUT or PATCH based on API design

                if (!response.success) {
                    throw new Error(response.message || "Failed to edit set.");
                }

                // Update the state with the edited set
                // Find the index of the set being edited
                const setIndexToEdit = currentExercise.sets.findIndex(set => set.id === this.state.currentSetId);
                if (setIndexToEdit !== -1) {
                    // Replace the old set with the new set data from the response
                    currentExercise.sets[setIndexToEdit] = {
                        id: this.state.currentSetId, // Keep the same ID
                        reps: response.reps.map(rep => ({ // Map updated rep data
                            id: rep.id, // Use IDs from response
                            weight: rep.weight,
                            difficulty: rep.difficulty,
                            order: rep.order
                        }))
                    };
                } else {
                    console.warn(`Edited set with ID ${this.state.currentSetId} not found in state after successful API call.`);
                    // If set not found in state, maybe refetch exercise or workout data?
                    // For simplicity, let's just re-render and hope it's consistent or handle in a fuller refetch.
                }


                this.state.editingSet = false; // Exit editing mode
                this.state.currentSetId = null; // Clear current set ID


            } else { // Adding a new set
                // Call API to add a new set
                response = await this.callApi(`addSet/${this.state.currentExerciseId}`, 'POST', repsToSave);

                if (!response.success) {
                    throw new Error(response.message || "Failed to add set.");
                }

                // Add the new set from the response to the state
                currentExercise.sets.push({
                    id: response.setId, // Use ID from response
                    reps: response.reps.map(rep => ({ // Map new rep data
                        id: rep.id, // Use IDs from response
                        weight: rep.weight,
                        difficulty: rep.difficulty,
                        order: rep.order
                    }))
                });
            }

            // Update counts and navigate back to the exercise detail view
            this.updateExerciseCount(); // Total sets count might change
            this.backToExercise(); // Navigate back

        } catch (error) {
            alert("Error saving set: " + error.message);
        }
    }


    static async deleteSet(exerciseId, setId) {
        if (!this.state.currentSessionId || !this.state.currentExerciseId) return; // Ensure active workout and exercise context

        const exercise = this.state.exercises.find(ex => ex.id === exerciseId);
        if (!exercise) {
            console.error(`Exercise with ID ${exerciseId} not found in state for set deletion.`);
            return;
        }
        const set = exercise.sets.find(s => s.id === setId);
        if (!set) {
            console.error(`Set with ID ${setId} not found in state for deletion.`);
            return;
        }


        if (!confirm("Delete this set? This cannot be undone.")) {
            return; // User cancelled
        }

        try {
            // Call API to delete the set
            const response = await this.callApi(`deleteSet/${setId}`, 'DELETE');
            if (!response.success) {
                throw new Error(response.message);
            }

            // Remove the set from the state
            exercise.sets = exercise.sets.filter(s => s.id !== setId);


            // Update UI elements
            this.updateExerciseCount(); // Total sets count will change
            this.renderSetsList(); // Re-render the sets list for the current exercise

            // If the deleted set was the one being viewed (unlikely as delete is from list view, but defensive)
            if (this.state.currentSetId === setId) {
                this.backToExercise(); // Go back to exercise view
            }

        } catch (error) {
            alert("Error deleting set: " + error.message);
        }
    }

    static viewSetDetails(exerciseId, setId) {
        if (!this.state.currentSessionId) return; // Ensure active workout

        const exercise = this.state.exercises.find(ex => ex.id === exerciseId);
        if (!exercise) {
            console.error(`Exercise with ID ${exerciseId} not found in state for viewing set details.`);
            return;
        }

        const set = exercise.sets.find(s => s.id === setId);
        if (!set) {
            console.error(`Set with ID ${setId} not found in state for viewing details.`);
            return;
        }

        // Set state for editing the set
        this.state.editingSet = true;
        this.state.currentExerciseId = exerciseId; // Ensure exercise context is kept
        this.state.currentExerciseName = exercise.name; // Store exercise name
        this.state.currentSetId = setId;

        // Populate currentReps state from the set's reps
        this.state.currentReps = set.reps.map(rep => ({
            // Use original IDs if they exist, otherwise generate temp ones (though API should provide them)
            id: rep.id || 'temp-rep-' + Date.now() + '-' + Math.random().toString(36).substr(2, 9),
            difficulty: rep.difficulty,
            weight: rep.weight,
            order: rep.order
        }));

        // Use helper to switch view
        this.showView('setDetailView');
        // Update title to show set context
        // Find the index of the set to get its number for the title
        const setIndex = exercise.sets.findIndex(s => s.id === setId);
        this.updateContextTitle(`Workout > ${this.state.workoutName} > ${this.state.currentExerciseName} > Set ${setIndex !== -1 ? setIndex + 1 : 'Details'}`);


        // Update UI elements for set detail view
        document.getElementById("setDetailTitle").textContent = `Set ${setIndex !== -1 ? setIndex + 1 : 'Details'} for ${this.state.currentExerciseName}`; // More specific title
        document.getElementById("setWeightInput").value = set.reps[0]?.weight || 20; // Set default weight input based on first rep weight


        this.renderRepsList(); // Render the reps list for this set
    }


    static updateExerciseCount() {
        let totalSets = 0;
        this.state.exercises.forEach(ex => totalSets += (ex.sets ? ex.sets.length : 0)); // Ensure sets is an array
        document.getElementById("exerciseCount").textContent = this.state.exercises.length;
        document.getElementById("totalSets").textContent = totalSets;
    }

    static renderExercisesList() {
        const container = document.getElementById("exercisesList");
        if (!container) return; // Defensive check
        container.innerHTML = "";
        if (this.state.exercises.length === 0) {
            container.innerHTML = '<div class="list-group-item text-muted">No exercises added yet</div>'; // Wrap in list-group-item for consistency
            return;
        }
        this.state.exercises.forEach(exercise => {
            const exerciseItem = document.createElement("div");
            exerciseItem.className = "list-group-item exercise-item";
            exerciseItem.innerHTML = `
                <div class="d-flex justify-content-between align-items-center">
                    <div>
                        <h6 class="mb-1">${exercise.name}</h6>
                        <small class="text-muted">${exercise.sets ? exercise.sets.length : 0} sets</small>
                    </div>
                    <div>
                        <button class="btn btn-sm btn-outline-primary me-2" onclick="GymTracker.viewExerciseDetails('${exercise.id}')">
                            View/Add Sets
                        </button>
                        <button class="btn btn-sm btn-outline-danger" onclick="GymTracker.deleteExercise('${exercise.id}')">
                            <i class="bi bi-trash"></i>
                        </button>
                    </div>
                </div>`;
            container.appendChild(exerciseItem);
        });
    }

    static renderSetsList() {
        const exercise = this.state.exercises.find(ex => ex.id === this.state.currentExerciseId);
        const container = document.getElementById("setsTableBody");
        if (!exercise || !container) { // Defensive checks
            if (container) container.innerHTML = ''; // Clear table body if exercise not found
            console.error(`Exercise with ID ${this.state.currentExerciseId} not found to render sets.`);
            return;
        }


        container.innerHTML = ""; // Clear existing rows
        if (!exercise.sets || exercise.sets.length === 0) { // Ensure sets array exists and check its length
            container.innerHTML = '<tr><td colspan="3" class="text-center text-muted">No sets recorded yet</td></tr>';
            return;
        }

        // Sort sets by start time if available, or by order/index
        const sortedSets = exercise.sets.sort((a, b) => {
            // Assuming sets might have a startTime property from the API
            if (a.startTime && b.startTime) {
                return new Date(a.startTime) - new Date(b.startTime);
            }
            // Fallback to assuming order matches index if no startTime
            // Note: This relies on the sets array order being consistent
            return exercise.sets.indexOf(a) - exercise.sets.indexOf(b);
        });


        sortedSets.forEach((set, index) => {
            // Ensure reps array exists and get its length
            const repCount = set.reps ? set.reps.length : 0;

            const setRow = document.createElement("tr");
            setRow.innerHTML = `
                <td>${index + 1}</td> <td>${repCount}</td>
                <td>
                    <button class="btn btn-sm btn-outline-info me-2" onclick="GymTracker.viewSetDetails('${exercise.id}', '${set.id}')">
                        Edit
                    </button>
                    <button class="btn btn-sm btn-outline-danger" onclick="GymTracker.deleteSet('${exercise.id}', '${set.id}')">
                        Delete
                    </button>
                </td>`;
            container.appendChild(setRow);
        });
    }


    static renderRepsList() {
        const container = document.getElementById("repsList");
        if (!container) return; // Defensive check
        container.innerHTML = "";

        if (!this.state.currentReps || this.state.currentReps.length === 0) { // Ensure currentReps exists and check its length
            container.innerHTML = '<div class="text-muted mb-2">No reps added yet</div>';
            return;
        }

        this.state.currentReps.forEach((rep, index) => {
            // Use rep.weight if available, otherwise fall back to the input value or 0
            const weight = rep.weight > 0 ? rep.weight : (parseFloat(document.getElementById("setWeightInput")?.value) || 0);

            const repItem = document.createElement("div");
            repItem.className = "list-group-item rep-item d-flex justify-content-between align-items-center"; // Added flex classes
            repItem.innerHTML = `
                <span>Rep ${index + 1}</span>
                <div class="rep-item-controls">
                     <span class="badge difficulty-badge ${rep.difficulty}"
                           onclick="GymTracker.updateRepDifficulty('${rep.id}')"
                           style="cursor: pointer;"> ${weight.toFixed(1)}kg (${rep.difficulty}) </span>
                    <button class="btn btn-sm btn-outline-secondary" onclick="GymTracker.updateRepWeight('${rep.id}')">
                        <i class="bi bi-pencil"></i>
                    </button>
                    <button class="btn btn-sm btn-outline-danger" onclick="GymTracker.deleteRep('${rep.id}')">
                        <i class="bi bi-trash"></i>
                    </button>
                </div>`;
            container.appendChild(repItem);
        });
    }

    static backToWorkout() {
        if (!this.state.currentSessionId) return; // Cannot go back if no workout is active

        // Clear exercise/set specific state
        this.state.currentExerciseId = null;
        this.state.currentExerciseName = null;
        this.state.currentSetId = null;
        this.state.currentReps = []; // Clear reps list
        this.state.editingSet = false; // Exit editing mode

        // Use helper to show the active workout view
        this.showView('activeWorkoutView');
        // Update the context title back to the workout level
        this.updateContextTitle('Workout > ' + (this.state.workoutName || 'Active Workout'));

        // Ensure elements within activeWorkoutView are visible as needed
        document.getElementById("exerciseMetricViews").style.display = "block"; // Ensure metrics are shown
        // The input area should also be visible if it's a flex child of activeWorkoutView

        this.renderExercisesList(); // Re-render the exercises list
        // Sets/Reps lists will be cleared/hidden as their views are hidden
    }

    static backToExercise() {
        if (!this.state.currentSessionId || !this.state.currentExerciseId) {
            // If somehow navigated here without an exercise, go back to workout
            this.backToWorkout();
            return;
        }

        // Clear set specific state
        this.state.currentSetId = null;
        this.state.currentReps = []; // Clear reps list
        this.state.editingSet = false; // Exit editing mode

        // Use helper to show the exercise detail view
        this.showView('exerciseDetailView');
        // Update the context title back to the exercise level
        this.updateContextTitle(`Workout > ${this.state.workoutName} > ${this.state.currentExerciseName}`);

        this.renderSetsList(); // Re-render the sets list for the current exercise
        // Reps list will be cleared/hidden as the set detail view is hidden
    }


    static async loadWorkoutHistory() {
        try {
            // showHistoryLoading(true); // Managed in the event listener now
            const response = await this.callApi('workoutHistory');
            if (!response.success) {
                throw new Error(response.message);
            }

            const sessionsList = document.getElementById('workoutSessionsList');
            if (!sessionsList) {
                console.error("Workout history sessions list element not found.");
                this.showHistoryLoading(false);
                return;
            }
            sessionsList.innerHTML = ''; // Clear previous history

            if (!response.sessions || response.sessions.length === 0) { // Check if sessions array exists and is not empty
                sessionsList.innerHTML = '<li class="list-group-item text-muted">No workout history found</li>';
                document.getElementById('historyLoading').style.display = 'none';
                document.getElementById('historyContent').style.display = 'block';
                return;
            }

            response.sessions.forEach(session => {
                const sessionItem = document.createElement('li');
                sessionItem.className = 'list-group-item d-flex justify-content-between align-items-center';
                sessionItem.innerHTML = `
                    <div>
                        <strong>${session.name}</strong>
                        <div class="text-muted small">
                            ${new Date(session.startTime).toLocaleDateString()} •
                            ${session.duration !== undefined ? Math.round(session.duration) + ' minutes' : 'N/A duration'} •
                            ${session.exerciseCount !== undefined ? session.exerciseCount + ' exercises' : 'N/A exercises'} •
                            ${session.totalSets !== undefined ? session.totalSets + ' sets' : 'N/A sets'}
                        </div>
                    </div>
                    <div>
                        <button class="btn btn-sm btn-outline-primary me-2" onclick="GymTracker.viewSessionDetails('${session.id}')">
                            <i class="bi bi-eye"></i> View
                        </button>
                        <button class="btn btn-sm btn-outline-danger" onclick="GymTracker.deleteSession('${session.id}')">
                            <i class="bi bi-trash"></i> Delete
                        </button>
                    </div>`;
                sessionsList.appendChild(sessionItem);
            });

            this.showHistoryLoading(false); // Hide loading, show content
        } catch (error) {
            this.showHistoryLoading(false);
            const sessionsList = document.getElementById('workoutSessionsList');
            if (sessionsList) {
                sessionsList.innerHTML = '<li class="list-group-item text-danger">Error loading history: ' + error.message + '</li>';
            }
            console.error('Error loading workout history:', error);
            document.getElementById('historyContent').style.display = 'block'; // Ensure content area is visible even with error
        }
    }

    static async viewSessionDetails(sessionId) {
        try {
            this.showHistoryLoading(true); // Show loading
            const response = await this.callApi(`workoutDetails/${sessionId}`);
            if (!response.success) {
                throw new Error(response.message);
            }

            const workoutSessionsList = document.getElementById('workoutSessionsList');
            const workoutDetails = document.getElementById('workoutDetails');
            const exercisesContainer = document.getElementById('sessionExercises');

            if (!workoutSessionsList || !workoutDetails || !exercisesContainer) {
                console.error("Required history modal elements not found.");
                this.showHistoryLoading(false);
                return;
            }


            workoutDetails.dataset.sessionId = sessionId; // Store session ID on the details div

            // Hide the sessions list and show the details area
            workoutSessionsList.style.display = 'none';
            workoutDetails.style.display = 'block';

            exercisesContainer.innerHTML = ''; // Clear previous details


            if (!response.session || !response.session.exercises || response.session.exercises.length === 0) {
                exercisesContainer.innerHTML = '<div class="alert alert-info">No exercises found for this session.</div>';
            } else {
                response.session.exercises.forEach(exercise => {
                    const exerciseCard = document.createElement('div');
                    exerciseCard.className = 'card mb-3';
                    exerciseCard.innerHTML = `
                          <div class="card-header d-flex justify-content-between align-items-center">
                              <h6 class="mb-0">${exercise.name}</h6>
                              <button class="btn btn-sm btn-outline-danger" onclick="GymTracker.deleteExerciseFromHistory('${exercise.id}')">
                                  <i class="bi bi-trash"></i> Delete
                              </button>
                          </div>
                          <div class="card-body">
                              <ul class="list-group">
                                  ${(exercise.sets || []).sort((a, b) => {
                        // Sort sets by start time if available, otherwise use a fallback
                        if (a.startTime && b.startTime) return new Date(a.startTime) - new Date(b.startTime);
                        // Fallback: maybe sort by the start time of the first rep if set doesn't have start time?
                        // Or just keep the order from the API? Let's sort by first rep time if set time not avail.
                        const aTime = a.reps && a.reps[0] && a.reps[0].startTime ? new Date(a.reps[0].startTime) : null;
                        const bTime = b.reps && b.reps[0] && b.reps[0].startTime ? new Date(b.reps[0].startTime) : null;
                        if (aTime && bTime) return aTime - bTime;
                        return 0; // Keep original order if no time info
                    })
                            .map((set, index) => {
                                const repDetails = (set.reps || []).sort((a, b) => a.order - b.order) // Sort reps by order
                                    .map(rep => `
                                               <span class="badge difficulty-badge ${rep.difficulty} me-1">
                                                   ${rep.weight !== undefined ? rep.weight : 'N/A'}kg (${rep.difficulty})
                                               </span>
                                            `).join('');

                                return `
                                            <li class="list-group-item d-flex justify-content-between align-items-center">
                                                <div>
                                                    <strong>Set: ${index + 1}</strong>
                                                    <div class="mt-1">${repDetails || 'No reps recorded'}</div>
                                                </div>
                                                <button class="btn btn-sm btn-outline-danger" onclick="GymTracker.deleteSetFromHistory('${set.id}')">
                                                    <i class="bi bi-trash"></i>
                                                </button>
                                            </li>`;
                            }).join('')}
                              </ul>
                          </div>`;
                    exercisesContainer.appendChild(exerciseCard);
                });
            }


            this.showHistoryLoading(false); // Hide loading, show content
        } catch (error) {
            this.showHistoryLoading(false);
            const workoutDetails = document.getElementById('workoutDetails');
            const exercisesContainer = document.getElementById('sessionExercises');
            const workoutSessionsList = document.getElementById('workoutSessionsList');

            if (workoutDetails && exercisesContainer && workoutSessionsList) {
                workoutSessionsList.style.display = 'none'; // Hide sessions list
                workoutDetails.style.display = 'block'; // Show details area
                exercisesContainer.innerHTML = '<div class="alert alert-danger">Error loading session details: ' + error.message + '</div>';
            } else {
                alert('Error loading session details: ' + error.message); // Fallback alert
            }
            console.error('Error loading session details:', error);
        }
    }

    static async deleteSession(sessionId) {
        if (!confirm("Delete this workout session and all its data? This cannot be undone.")) {
            return; // User cancelled
        }

        try {
            // Show loading indicator in the modal
            this.showHistoryLoading(true);

            const response = await this.callApi(`deleteWorkout/${sessionId}`, 'DELETE');
            if (!response.success) {
                throw new Error(response.message);
            }

            // Reload the history list after deletion
            await this.loadWorkoutHistory();

        } catch (error) {
            this.showHistoryLoading(false); // Hide loading even on error
            alert("Error deleting session: " + error.message);
            console.error('Error deleting session:', error);
        }
    }

    static async deleteExerciseFromHistory(exerciseId) {
        const workoutDetails = document.getElementById('workoutDetails');
        const sessionId = workoutDetails?.dataset.sessionId; // Get session ID from the element
        if (!sessionId) {
            console.error("Session ID not found on workout details element.");
            return;
        }

        if (!confirm("Delete this exercise from history? This cannot be undone.")) {
            return; // User cancelled
        }

        try {
            // Show loading indicator in the modal
            this.showHistoryLoading(true);

            const response = await this.callApi(`deleteExerciseFromHistory/${exerciseId}`, 'DELETE');
            if (!response.success) {
                throw new Error(response.message);
            }

            // Reload the session details after deletion
            await this.viewSessionDetails(sessionId);

        } catch (error) {
            this.showHistoryLoading(false); // Hide loading even on error
            alert("Error deleting exercise from history: " + error.message);
            console.error('Error deleting exercise from history:', error);
        }
    }

    static async deleteSetFromHistory(setId) {
        const workoutDetails = document.getElementById('workoutDetails');
        const sessionId = workoutDetails?.dataset.sessionId; // Get session ID from the element
        if (!sessionId) {
            console.error("Session ID not found on workout details element.");
            return;
        }

        if (!confirm("Delete this set from history? This cannot be undone.")) {
            return; // User cancelled
        }

        try {
            // Show loading indicator in the modal
            this.showHistoryLoading(true);

            const response = await this.callApi(`deleteSetFromHistory/${setId}`, 'DELETE');
            if (!response.success) {
                throw new Error(response.message);
            }

            // Reload the session details after deletion
            await this.viewSessionDetails(sessionId);

        } catch (error) {
            this.showHistoryLoading(false); // Hide loading even on error
            alert("Error deleting set from history: " + error.message);
            console.error('Error deleting set from history:', error);
        }
    }


    static showHistoryLoading(isLoading) {
        const historyLoading = document.getElementById('historyLoading');
        const historyContent = document.getElementById('historyContent');

        if (!historyLoading || !historyContent) {
            console.error("History loading or content element not found.");
            return;
        }

        if (isLoading) {
            historyLoading.style.display = 'block';
            historyContent.style.display = 'none';
        } else {
            historyLoading.style.display = 'none';
            historyContent.style.display = 'block';
        }
    }
}