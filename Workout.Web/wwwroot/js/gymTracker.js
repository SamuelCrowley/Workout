class GymTracker {
    static config = {
        difficultyOptions: [],
        antiforgeryToken: ''
    };

    static state = {
        currentSessionId: null,
        currentExerciseId: null,
        currentSetId: null,
        workoutStartTime: null,
        timerInterval: null,
        restTimerEndTime: null,
        exercises: [],
        currentReps: [],
        editingSet: false
    };

    static initialize(config) {
        this.config = config;
        this.bindEvents();
        this.checkGymUserExists();
        this.checkActiveWorkout();
    }

    static bindEvents() {
        document.getElementById("startWorkoutBtn")?.addEventListener("click", () => this.startWorkout());
        document.getElementById("createGymUserBtn")?.addEventListener("click", () => this.createGymUser());
        document.getElementById("endWorkoutBtn")?.addEventListener("click", () => this.endWorkout());
        document.getElementById("addExerciseBtn")?.addEventListener("click", () => this.addExercise());
        document.getElementById("addSetBtn")?.addEventListener("click", () => this.addSet());
        document.getElementById("backToWorkoutBtn")?.addEventListener("click", () => this.backToWorkout());
        document.getElementById("backToExercisesBtn")?.addEventListener("click", () => this.backToExercise());
        document.getElementById("addRepsBtn")?.addEventListener("click", () => this.addReps());
        document.getElementById("clearRepsBtn")?.addEventListener("click", () => this.clearReps());
        document.getElementById("saveSetBtn")?.addEventListener("click", () => this.saveSet());
        document.getElementById("startRestTimerBtn")?.addEventListener("click", () => this.startRestTimer());
        document.getElementById("stopRestTimerBtn")?.addEventListener("click", () => this.stopRestTimer());

        document.getElementById("restTimerDuration")?.addEventListener("change", (e) => {
            this.state.restTimerRemaining = parseInt(e.target.value);
            this.stopRestTimer();

            const minutes = Math.floor(this.state.restTimerRemaining / 60).toString().padStart(2, '0');
            const seconds = (this.state.restTimerRemaining % 60).toString().padStart(2, '0');
            document.getElementById("restTimerDisplay").textContent = `Rest: ${minutes}:${seconds}`;
        });

        document.getElementById('viewHistoryBtn')?.addEventListener('click', async () => {
            const historyModalEl = document.getElementById('historyModal');

            if (historyModalEl.dataset.processing === 'true') {
                return;
            }

            historyModalEl.dataset.processing = 'true';

            let modal = bootstrap.Modal.getInstance(historyModalEl);
            if (!modal) {
                modal = new bootstrap.Modal(historyModalEl, {
                    keyboard: true,
                    backdrop: true
                });
            }

            const cleanup = () => {
                document.querySelectorAll('.modal-backdrop').forEach(el => el.remove());
                document.body.classList.remove('modal-open');
                document.body.style.paddingRight = '';
                historyModalEl.dataset.processing = 'false';

                historyModalEl.removeEventListener('hidden.bs.modal', cleanup);
            };

            historyModalEl.addEventListener('hidden.bs.modal', cleanup, { once: true });

            try {
                modal.show();
                document.getElementById('workoutDetails').style.display = 'none';
                await this.loadWorkoutHistory();
            } catch (error) {
                cleanup();
            } finally {
                setTimeout(() => {
                    historyModalEl.dataset.processing = 'false';
                }, 500);
            }
        });

        const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
        tooltipTriggerList.forEach(tooltipTriggerEl => new bootstrap.Tooltip(tooltipTriggerEl));
    }

    static async callApi(endpoint, method = 'GET', body = null) {
        const headers = {
            'Accept': 'application/json',
            'RequestVerificationToken': this.config.antiforgeryToken
        };

        if (method !== 'GET') {
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
                const errorData = await response.json();
                throw new Error(errorData.message || `Error encountered: ${response.status}`);
            }

            return await response.json();
        } catch (error) {
            throw error;
        }
    }

    static async checkGymUserExists() {
        try {
            const existsResponse = await this.callApi('exists');

            document.getElementById("createGymUserBtn").style.display = existsResponse.exists ? "none" : "inline-block";
            document.getElementById("workoutNameContainer").style.display = existsResponse.exists ? "block" : "none";

            if (existsResponse.exists) {
                const activeResponse = await this.checkActiveWorkout();
                document.getElementById("startWorkoutBtn").style.display = activeResponse.hasActiveWorkout ? "none" : "inline-block";
            }
        } catch (error) {
            alert("Error checking gym user status: " + error.message);
        }
    }

    static async checkActiveWorkout() {
        try {
            const response = await this.callApi('activeWorkout');

            if (response.success && response.hasActiveWorkout) {
                const workout = response.workout;
                this.state.currentSessionId = workout.id;
                this.state.workoutStartTime = new Date(workout.startTime).toISOString();
                this.state.exercises = workout.exercises.map(ex => ({
                    id: ex.id,
                    name: ex.name,
                    sets: ex.sets.map(set => ({
                        id: set.id,
                        reps: set.reps.map(rep => ({
                            id: rep.id,
                            weight: rep.weight,
                            difficulty: rep.difficulty,
                            order: rep.order
                        }))
                    }))
                }));

                document.getElementById("initialView").style.display = "none";
                document.getElementById("activeWorkoutView").style.display = "block";
                document.getElementById("currentExercises").style.display = "block";
                document.getElementById("workoutNameDisplay").textContent = workout.name;
                document.getElementById("restTimerContainer").style.display = "block";

                this.startTimer();
                this.renderExercisesList();
                this.updateExerciseCount();
            }
            return response;
        } catch (error) {
            alert("Error checking active workout: " + error.message);
        }
    }

    static async createGymUser() {
        try {
            await this.callApi('createGymUser', 'POST');

            alert("Gym user account created!");

            document.getElementById("startWorkoutBtn").style.display = "inline-block";
            document.getElementById("createGymUserBtn").style.display = "none";
            document.getElementById("workoutNameContainer").style.display = "block";
        } catch (error) {
            alert("Error creating gym user: " + error.message);
        }
    }

    static async startWorkout() {
        const workoutName = document.getElementById("workoutNameInput").value.trim();

        if (!workoutName) {
            alert("Please enter a workout name");
            return;
        }

        try {
            const response = await this.callApi('startWorkout', 'POST', workoutName);
            if (!response.success) {
                throw new Error(response.message);
            }

            this.resetWorkoutData();
            this.state.currentSessionId = response.sessionId;
            this.state.workoutStartTime = new Date().toISOString().replace('Z', '');
            this.startTimer();
            document.getElementById("initialView").style.display = "none";
            document.getElementById("activeWorkoutView").style.display = "block";
            document.getElementById("currentExercises").style.display = "block";
            document.getElementById("workoutNameDisplay").textContent = workoutName;
            document.getElementById("restTimerContainer").style.display = "block";
            this.state.restTimerRemaining = parseInt(document.getElementById("restTimerDuration").value);
        } catch (error) {
            alert("Error starting workout: " + error);
        }
    }

    static resetWorkoutData() {
        this.state.exercises = [];
        this.state.currentReps = [];
        this.state.currentExerciseId = null;
        this.state.currentSetId = null;
        this.updateExerciseCount();

        document.getElementById("exercisesList").innerHTML = "";
        document.getElementById("setsTableBody").innerHTML = "";

        document.getElementById("repsList").innerHTML = "";
        document.getElementById("exerciseNameInput").value = "";
        document.getElementById("setWeightInput").value = "20";
    }

    static async endWorkout() {
        if (!confirm("Are you sure you want to end this workout?")) {
            return
        };

        try {
            const response = await this.callApi(`endWorkout/${this.state.currentSessionId}`, 'POST');
            if (!response.success) {
                throw new Error(response.message);
            }

            this.stopTimer();
            this.stopRestTimer();
            this.state.currentSessionId = null;
            this.state.workoutStartTime = null;
            this.resetWorkoutData();

            document.getElementById("activeWorkoutView").style.display = "none";
            document.getElementById("currentExercises").style.display = "none";
            document.getElementById("exerciseDetailView").style.display = "none";
            document.getElementById("setDetailView").style.display = "none";
            document.getElementById("initialView").style.display = "block";
            document.getElementById("restTimerContainer").style.display = "none";
            document.getElementById("workoutNameInput").value = "";

            document.getElementById("startWorkoutBtn").style.display = "inline-block";
        } catch (error) {
            alert("Error ending workout: " + error.message);
        }
    }

    static startTimer() {
        this.stopTimer();

        // SEC 29-Apr-2025 - Absolute horrorshow date conversions, putting this to the side to focus on actual functionality but aware this code should be improved.
        const updateTimer = () => {
            const now = Date.now();
            const utc = new Date(now).toISOString();

            const localTimeAsUTC = new Date(utc.replace('Z', '')).getTime();
            const workoutTimeAsUTC = new Date(this.state.workoutStartTime).getTime();

            const diff = localTimeAsUTC - workoutTimeAsUTC;

            const hours = Math.floor(diff / 3600000);
            const minutes = Math.floor((diff % 3600000) / 60000);
            const seconds = Math.floor((diff % 60000) / 1000);

            document.getElementById("workoutTimer").textContent =
                `${String(hours).padStart(2, '0')}:` +
                `${String(minutes).padStart(2, '0')}:` +
                `${String(seconds).padStart(2, '0')}`;
        };

        updateTimer();
        this.state.timerInterval = setInterval(updateTimer, 1000);
    }

    static stopTimer() {
        if (this.state.timerInterval) {
            clearInterval(this.state.timerInterval);
            this.state.timerInterval = null;
        }
    }

    static startRestTimer() {
        this.stopRestTimer();

        const duration = parseInt(document.getElementById("restTimerDuration").value);

        const endTime = Date.now() + (duration * 1000);
        this.state.restTimerEndTime = endTime;

        this.updateRestTimerDisplay();
        this.state.restTimerInterval = setInterval(() => this.updateRestTimerDisplay(), 1000);
    }

    static updateRestTimerDisplay() {
        if (!this.state.restTimerEndTime) {
            return;
        }

        const now = Date.now();
        const remaining = Math.max(0, this.state.restTimerEndTime - now);

        const minutes = Math.floor(remaining / 60000);
        const seconds = Math.floor((remaining % 60000) / 1000);

        document.getElementById("restTimerDisplay").textContent =
            `Rest: ${String(minutes).padStart(2, '0')}:${String(seconds).padStart(2, '0')}`;

        if (remaining <= 0) {
            this.stopRestTimer();
        }
    }

    static stopRestTimer() {
        if (this.state.restTimerInterval) {
            clearInterval(this.state.restTimerInterval);
            this.state.restTimerInterval = null;
        }
        this.state.restTimerEndTime = null;
        document.getElementById("restTimerDisplay").textContent = "Rest: 00:00";
    }

    static async addExercise() {
        const exerciseName = document.getElementById("exerciseNameInput").value.trim();
        if (!exerciseName) {
            alert("Please enter an exercise name");
            return;
        }

        try {
            const response = await this.callApi(`addExercise/${this.state.currentSessionId}`, 'POST', exerciseName);
            if (!response.success) {
                throw new Error(response.message);
            }

            this.state.exercises.push({
                id: response.exerciseId,
                name: exerciseName,
                sets: []
            });
            this.updateExerciseCount();
            this.renderExercisesList();
            document.getElementById("exerciseNameInput").value = "";
        } catch (error) {
            alert("Error adding exercise: " + error.message);
        }
    }

    static async deleteExercise(exerciseId) {
        if (!confirm("Delete this exercise and all its sets?")) return;

        try {
            const response = await this.callApi(`deleteExercise/${exerciseId}`, 'DELETE');
            if (!response.success) {
                throw new Error(response.message);
            }

            this.state.exercises = this.state.exercises.filter(ex => ex.id !== exerciseId);
            this.updateExerciseCount();
            this.renderExercisesList();

            if (this.state.currentExerciseId === exerciseId) {
                this.backToWorkout();
            }
        } catch (error) {
            alert("Error deleting exercise: " + error.message);
        }
    }

    static viewExerciseDetails(exerciseId) {
        const exercise = this.state.exercises.find(ex => ex.id === exerciseId);
        if (!exercise) {
            return;
        }
        this.state.currentExerciseId = exerciseId;
        document.getElementById("exerciseDetailTitle").textContent = exercise.name;
        document.getElementById("currentExercises").style.display = "none";
        document.getElementById("exerciseDetailView").style.display = "block";
        document.getElementById("setDetailView").style.display = "none";
        this.renderSetsList();
    }

    static addSet() {
        this.state.currentReps = [];
        this.state.editingSet = false;
        document.getElementById("setDetailTitle").textContent = document.getElementById("exerciseDetailTitle").textContent;
        document.getElementById("exerciseDetailView").style.display = "none";
        document.getElementById("setDetailView").style.display = "block";
        document.getElementById("setWeightInput").value = "20";
        this.renderRepsList();
    }

    static addReps() {
        const repCount = parseInt(document.getElementById("repCountInput").value);
        const defaultDifficulty = document.getElementById("defaultDifficultyInput").value;
        const defaultWeight = parseFloat(document.getElementById("setWeightInput").value) || 0;
        

        for (let i = 0; i < repCount; i++) {
            this.state.currentReps.push({
                id: 'rep-' + Date.now() + '-' + i,
                difficulty: defaultDifficulty,
                weight: defaultWeight,
                order: this.state.currentReps.length + 1
            });
        }
        this.renderRepsList();
    }

    static clearReps() {
        if (confirm("Clear all reps?")) {
            this.state.currentReps = [];
            this.renderRepsList();
        }
    }

    static updateRepDifficulty(repId) {
        const repIndex = this.state.currentReps.findIndex(rep => rep.id === repId);
        if (repIndex === -1) {
            return;
        }

        const currentIndex = this.config.difficultyOptions.indexOf(this.state.currentReps[repIndex].difficulty);
        const nextIndex = (currentIndex + 1) % this.config.difficultyOptions.length;
        this.state.currentReps[repIndex].difficulty = this.config.difficultyOptions[nextIndex];
        this.renderRepsList();
    }

    static updateRepWeight(repId) {
        const repIndex = this.state.currentReps.findIndex(rep => rep.id === repId);
        if (repIndex === -1) {
            return;
        }

        const currentWeight = this.state.currentReps[repIndex].weight || document.getElementById("setWeightInput").value;
        const newWeight = prompt("Enter new weight:", currentWeight);

        if (newWeight !== null && !isNaN(newWeight)) {
            this.state.currentReps[repIndex].weight = parseFloat(newWeight);
            this.renderRepsList();
        }
    }

    static deleteRep(repId) {
        this.state.currentReps = this.state.currentReps.filter(rep => rep.id !== repId);
        this.state.currentReps.forEach((rep, index) => rep.order = index + 1);
        this.renderRepsList();
    }

    static async saveSet() {
        const defaultWeight = parseFloat(document.getElementById("setWeightInput").value);
        if (isNaN(defaultWeight)) {
            alert("Please enter valid weight");
            return;
        }

        this.state.currentReps.forEach(rep => {
            if (!rep.weight || isNaN(rep.weight)) rep.weight = 0;
        });

        if (this.state.currentReps.length === 0) {
            alert("Add at least one rep");
            return;
        }

        try {
            const repsToSave = this.state.currentReps.map((rep, index) => ({
                Weight: rep.weight,
                Difficulty: rep.difficulty,
                Order: index + 1
            }));

            let response;

            if (this.state.editingSet) {
                response = await this.callApi(`editSet/${this.state.currentSetId}`, 'PATCH', repsToSave);

                const exerciseIndex = this.state.exercises.findIndex(ex => ex.id === this.state.currentExerciseId);
                if (exerciseIndex !== -1) {
                    const setIndex = this.state.exercises[exerciseIndex].sets.findIndex(set => set.id === this.state.currentSetId);
                    if (setIndex !== -1) {
                        this.state.exercises[exerciseIndex].sets.splice(setIndex, 1); //SEC 29-Apr-2025 - Splice is like RemoveAt();
                    }
                }

                this.state.editingSet = false;
            }
            else {
                response = await this.callApi(`addSet/${this.state.currentExerciseId}`, 'POST', repsToSave);
            }

            if (!response.success) {
                throw new Error(response.message);
            }

            const exerciseIndex = this.state.exercises.findIndex(ex => ex.id === this.state.currentExerciseId);
            if (exerciseIndex === -1) {
                return;
            }

            this.state.exercises[exerciseIndex].sets.push({
                id: response.setId,
                reps: response.reps.map(rep => ({
                    id: rep.id,
                    weight: rep.weight,
                    difficulty: rep.difficulty,
                    order: rep.order
                }))
            });

            this.updateExerciseCount();
            this.backToExercise();
        } catch (error) {
            alert("Error saving set: " + error.message);
        }
    }

    static async deleteSet(exerciseId, setId) {
        if (!confirm("Delete this set?")) return;

        try {
            const response = await this.callApi(`deleteSet/${setId}`, 'DELETE');
            if (!response.success) {
                throw new Error(response.message);
            }

            const exerciseIndex = this.state.exercises.findIndex(ex => ex.id === exerciseId);
            if (exerciseIndex === -1) {
                return;
            }

            const setIndex = this.state.exercises[exerciseIndex].sets.findIndex(set => set.id === setId);
            if (setIndex === -1) {
                return;
            }

            this.state.exercises[exerciseIndex].sets.splice(setIndex, 1);
            this.updateExerciseCount();
            this.renderSetsList();
        } catch (error) {
            alert("Error deleting set: " + error.message);
        }
    }

    static viewSetDetails(exerciseId, setId) {
        const exercise = this.state.exercises.find(ex => ex.id === exerciseId);
        if (!exercise) {
            return;
        }

        const set = exercise.sets.find(s => s.id === setId);
        if (!set) {
            return;
        }

        this.state.editingSet = true;
        this.state.currentExerciseId = exerciseId;
        this.state.currentSetId = setId;
        document.getElementById("setDetailTitle").textContent = exercise.name;
        document.getElementById("setWeightInput").value = set.reps[0]?.weight || 20;
        this.state.currentReps = set.reps.map(rep => ({
            id: rep.id,
            difficulty: rep.difficulty,
            weight: rep.weight,
            order: rep.order
        }));
        document.getElementById("exerciseDetailView").style.display = "none";
        document.getElementById("setDetailView").style.display = "block";
        this.renderRepsList();
    }

    static updateExerciseCount() {
        let totalSets = 0;
        this.state.exercises.forEach(ex => totalSets += ex.sets.length);
        document.getElementById("exerciseCount").textContent = this.state.exercises.length;
        document.getElementById("totalSets").textContent = totalSets;
    }

    static renderExercisesList() {
        const container = document.getElementById("exercisesList");
        container.innerHTML = "";
        if (this.state.exercises.length === 0) {
            container.innerHTML = '<div class="text-muted">No exercises added yet</div>';
            return;
        }
        this.state.exercises.forEach(exercise => {
            const exerciseItem = document.createElement("div");

            let deleteOnClick = this.getOnClick("deleteExercise", exercise.id);
            let deleteButton = this.getDeleteButton(deleteOnClick);
            let viewOnClick = this.getOnClick("viewExerciseDetails", exercise.id);
            let viewButton = this.getViewButton(viewOnClick);

            exerciseItem.className = "list-group-item exercise-item";
            exerciseItem.innerHTML = `
                <div class="d-flex justify-content-between align-items-center">
                    <div>
                        <h6 class="mb-1">${exercise.name}</h6>
                        <h6 class="mb-1">${exercise.sets.length} sets</small>
                    </div>
                    <div>
                        ${viewButton}
                        ${deleteButton}
                    </div>
                </div>`;
            container.appendChild(exerciseItem);
        });
    }

    static renderSetsList() {
        const exercise = this.state.exercises.find(ex => ex.id === this.state.currentExerciseId);
        if (!exercise) {
            return;
        }

        const container = document.getElementById("setsTableBody");
        container.innerHTML = "";
        if (exercise.sets.length === 0) {
            container.innerHTML = '<tr><td colspan="3" class="text-center txt-primary">No sets recorded yet</td></tr>';
            return;
        }
        exercise.sets.forEach((set, index) => {
            const setRow = document.createElement("tr");

            let joinedRef = `${exercise.id}', '${set.id}`;

            let deleteOnClick = this.getOnClick("deleteSet", joinedRef);
            let deleteButton = this.getDeleteButton(deleteOnClick);

            let viewOnClick = this.getOnClick("viewSetDetails", joinedRef);
            let viewButton = this.getViewButton(viewOnClick);

            setRow.innerHTML = `
                <td>${index + 1}</td>
                <td>${set.reps.length}</td>
                <td>
                    ${viewButton}
                    ${deleteButton}
                </td>`;
            container.appendChild(setRow);
        });
    }

    static renderRepsList() {
        const container = document.getElementById("repsList");
        container.innerHTML = "";
        if (this.state.currentReps.length === 0) {
            container.innerHTML = '<div class="text-muted mb-2">No reps added yet</div>';
            return;
        }
        this.state.currentReps.forEach((rep, index) => {
            const weight = rep.weight || document.getElementById("setWeightInput").value;
            const repItem = document.createElement("div");

            let deleteOnClick = this.getOnClick("deleteRep", rep.id);
            let deleteButton = this.getDeleteButton(deleteOnClick);
            let editOnClick = this.getOnClick("updateRepWeight", rep.id);
            let editButton = this.getEditButton(editOnClick);

            repItem.className = "list-group-item rep-item";
            repItem.innerHTML = `
                <span>Rep ${index + 1}</span>
                <div class="rep-item-controls">
                    <span class="badge ${rep.difficulty} difficulty-badge"
                          onclick="GymTracker.updateRepDifficulty('${rep.id}')">
                        ${rep.difficulty} (${weight})
                    </span>
                    ${editButton}
                    ${deleteButton}
                </div>`;
            container.appendChild(repItem);
        });
    }

    static backToWorkout() {
        document.getElementById("exerciseDetailView").style.display = "none";
        document.getElementById("setDetailView").style.display = "none";
        document.getElementById("currentExercises").style.display = "block";
        this.state.currentExerciseId = null;
        this.state.currentSetId = null;
        this.renderExercisesList();
    }

    static backToExercise() {
        document.getElementById("setDetailView").style.display = "none";
        document.getElementById("exerciseDetailView").style.display = "block";
        this.state.currentSetId = null;
        this.renderSetsList();
    }

    static async loadWorkoutHistory() {
        try {
            this.showHistoryLoading(true);
            const response = await this.callApi('workoutHistory');
            if (!response.success) {
                throw new Error(response.message);
            }

            const sessionsList = document.getElementById('workoutSessionsList');
            sessionsList.innerHTML = '';
            if (response.sessions.length === 0) {
                sessionsList.innerHTML = '<li class="list-group-item txt-primary">No workout history found</li>';
                return;
            }

            response.sessions.forEach(session => {

                let deleteOnClick = this.getOnClick("deleteSession", session.classRef);
                let deleteButton = this.getDeleteButton(deleteOnClick);

                let viewOnClick = this.getOnClick("viewSessionDetails", session.classRef);
                let viewButton = this.getViewButton(viewOnClick);

                const sessionItem = document.createElement('li');
                sessionItem.className = 'list-group-item d-flex justify-content-between align-items-center';
                sessionItem.innerHTML = `
                <div>
                    <h5>${session.gymSessionType}</h5>
                    <div class="txt-primary">
                        ${new Date(session.startTime).toLocaleDateString()}
                        ${Math.round(session.durationMinutes)} minutes
                        ${session.exerciseCount} exercises
                        ${session.totalSets} sets
                    </div>
                </div>
                <div style="display: flex; align-items: center; gap: 10px;">
                    ${viewButton}
                    ${deleteButton}
                </div>`;
                sessionsList.appendChild(sessionItem);
            });

            this.showHistoryLoading(false);
        } catch (error) {
            this.showHistoryLoading(false);
            alert('Error loading workout history: ' + error.message);
        }
    }

    static async viewSessionDetails(sessionId) {
        try {
            //this.showHistoryLoading(true);
            const response = await this.callApi(`workoutDetails/${sessionId}`);
            if (!response.success) {
                throw new Error(response.message);
            }

            const workoutDetails = document.getElementById('workoutDetails');
            workoutDetails.dataset.sessionId = sessionId;
            const exercisesContainer = document.getElementById('sessionExercises');
            exercisesContainer.innerHTML = '';

            let deleteSetFromHistoryMethod = 'deleteSetFromHistory';

            response.session.exercises.forEach(exercise => {
                let deleteExerciseOnClick = this.getOnClick("deleteExerciseFromHistory", exercise.id);
                let deleteExerciseButton = this.getDeleteButton(deleteExerciseOnClick);

                const exerciseCard = document.createElement('div');
                exerciseCard.className = 'card mb-3';
                exerciseCard.innerHTML = `
                    <div class="card-header d-flex justify-content-between align-items-center bg-quartus">
                        <h2 class="mb-0">${exercise.name}</h2>
                        ${deleteExerciseButton}
                    </div>
                    <div class="card-body bg-tertius">
                        <ul class="list-group">
                            ${exercise.sets.map(set => {
                                let deleteSetOnClick = this.getOnClick(deleteSetFromHistoryMethod, set.id);
                                let deleteSetButton = this.getDeleteButton(deleteSetOnClick);
                                return  `<li class="list-group-item d-flex justify-content-between align-items-center txt-primary">
                                    <div>
                                        <strong>Set</strong>
                                        <div>
                                            ${set.reps.sort((a, b) => a.order - b.order).map(rep => `
                                                <span class="badge ${rep.difficulty} me-1 txt-primary">
                                                    ${rep.weight} (${rep.difficulty})
                                                </span>
                                            `).join('')}
                                        </div>
                                    </div>
                                    ${deleteSetButton}
                                </li>`
                            }).join('')}
                        </ul>
                    </div>`;
                exercisesContainer.appendChild(exerciseCard);
            });

            document.getElementById('workoutDetails').style.display = 'block';
        } catch (error) {
            alert('Error loading session details: ' + error.message);
        }
    }

    static async deleteSession(sessionId) {
        if (!confirm('Delete this entire workout session?')) return;

        try {
            const response = await this.callApi(`deleteSession/${sessionId}`, 'DELETE');
            if (!response.success) {
                throw new Error(response.message);
            }

            alert('Session deleted successfully');
            await this.loadWorkoutHistory();
            document.getElementById('workoutDetails').style.display = 'none';
        } catch (error) {
            alert('Error deleting session: ' + error.message);
        }
    }

    static async deleteExerciseFromHistory(exerciseId) {
        if (!confirm('Delete this exercise and all its sets?')) return;

        try {
            const response = await this.callApi(`deleteExercise/${exerciseId}`, 'DELETE');
            if (!response.success) {
                throw new Error(response.message);
            }

            alert('Exercise deleted successfully');
            const workoutDetails = document.getElementById('workoutDetails');
            if (workoutDetails.style.display === 'block') {
                await this.viewSessionDetails(workoutDetails.dataset.sessionId);
            }
        } catch (error) {
            alert('Error deleting exercise: ' + error.message);
        }
    }

    static async deleteSetFromHistory(setId) {
        if (!confirm('Delete this set?')) return;

        try {
            const response = await this.callApi(`deleteSet/${setId}`, 'DELETE');
            if (!response.success) {
                throw new Error(response.message);
            }

            alert('Set deleted successfully');
            const workoutDetails = document.getElementById('workoutDetails');
            if (workoutDetails.style.display === 'block') {
                await this.viewSessionDetails(workoutDetails.dataset.sessionId);
            }
        } catch (error) {
            alert('Error deleting set: ' + error.message);
        }
    }

    static showHistoryLoading(show) {
        document.getElementById('historyLoading').style.display = show ? 'block' : 'none';
        document.getElementById('historyContent').style.display = show ? 'none' : 'block';
    }

    static getDeleteButton(onClick) {
        return `<button class="btn btn-warning" ${onClick}>
            <i class="bi bi-trash"></i> 
        </button>`;
    }

    static getViewButton(onClick) {
        return `<button class="btn btn-standard" ${onClick}>
            <i class="bi bi-eye"></i> 
        </button>`;
    }

    static getEditButton(onClick) {
        return `<button class="btn btn-standard" ${onClick}">
            <i class="bi bi-pencil"></i> 
        </button>`;
    }

    static getOnClick(methodName, reference) {
        return `onClick="GymTracker.${methodName}('${reference}')"`
    }
}