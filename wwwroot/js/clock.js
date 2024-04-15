let timer;
let minutes = enteredTime;
let seconds = 0;
let isPaused = true;
let title = document.querySelector(".wrapper .logo");

const minutesElement =
	document.querySelector('.minutes');
const secondsElement =
	document.querySelector('.seconds');

function updateTimerUI(minutes, seconds){
	minutesElement.innerHTML = String(minutes).padStart(2, '0') + " <span>MINUTES</span>";
	secondsElement.innerHTML = String(seconds).padStart(2, '0') + " <span>SECONDS</span>";
}

updateTimerUI(minutes, seconds);

function startTimer() {
	clearInterval(timer);
	updateTimer();
	timer = setInterval(updateTimer, 1000);
}

function updateTimer() {
	updateTimerUI(minutes, seconds);
	if (minutes === 0 && seconds === 0) {
		clearInterval(timer);
		title.textContent = "Break time";
		breakTimer();
	}
	else if (!isPaused) {
		if (seconds > 0) {
			seconds--;
		} else {
			seconds = 59;
			minutes--;
		}
	}
}

function breakTimer() {
	seconds = 0;
	clearInterval(timer);
	updateBreak();
	timer = setInterval(updateBreak, 1000);
}

function updateBreak() {
	updateTimerUI(breakMin, seconds);
	if (breakMin === 0 && seconds === 0) {
		clearInterval(timer);
		title.textContent = "Pomodoro clock";
		restartTimer();
	}
	else if (!isPaused) {
		if (seconds > 0) {
			seconds--;
		} else {
			seconds = 59;
			breakMin--;
		}
	}
}

function togglePauseResume() {
	const pauseResumeButton =
		document.querySelector('.start');
	isPaused = !isPaused;

	if (isPaused) {
		clearInterval(timer);
		pauseResumeButton.textContent = 'Resume';
	} else {
		startTimer();
		pauseResumeButton.textContent = 'Pause';
	}
}

function restartTimer() {
	clearInterval(timer);
	var result = getMinutesSeconds(enteredTime);
	minutes = result.minutes;
	seconds = result.seconds;
	title.textContent = "Pomodoro clock";
	isPaused = true;
	breakMin = 1;
	updateTimerUI(minutes, seconds);
	const pauseResumeButton =
		document.querySelector('.start');
	pauseResumeButton.textContent = 'Start';
	startTimer();
}

function chooseTime() {
	const newTime = prompt('Enter new time in minutes:');
	if (!isNaN(newTime) && newTime > 0) {
		enteredTime = newTime;
		var result = getMinutesSeconds(enteredTime);
		minutes = result.minutes;
		seconds = result.seconds;
		isPaused = true;
		updateTimerUI(minutes, seconds);
		clearInterval(timer);
		const pauseResumeButton =
			document.querySelector('.start');
		pauseResumeButton.textContent = 'Start';
	} else {
		alert('Invalid input. Please enter' +
			' a valid number greater than 0.');
	}
}

function getMinutesSeconds(floatNumber) {
	// Multiply by 60 to convert to seconds (cast to integer to truncate decimals)
	const totalSeconds = Math.floor(floatNumber * 60);
  
	// Get minutes by dividing total seconds by 60 (integer division)
	const minutes = Math.floor(totalSeconds / 60);
  
	// Get seconds by finding the remainder after dividing by 60
	const seconds = totalSeconds % 60;
  
	return { minutes, seconds };
}
