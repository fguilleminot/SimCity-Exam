using System;

public class TimeManager {
	
	public double speedTime;
	public float refreshTime;
	
	private DateTime timeOfLaunching;
	
	public TimeManager(double speed = 1.0, float refresh = 0.5f)
	{
		speedTime = speed;
		refreshTime = refresh;
		timeOfLaunching = System.DateTime.Now;
	}
	
	// Update is called once per frame
	public void Update () {
		timeOfLaunching = timeOfLaunching.AddHours(speedTime);
	}
	
	public DateTime getTime()
	{
		return this.timeOfLaunching;
	}
	
	public bool changeMonth(DateTime previous, DateTime next)
	{
		return (previous.Month != next.Month);
	}
	
	public bool changeYear(DateTime previous, DateTime next)
	{
		return (previous.Year != next.Year);
	}
}
