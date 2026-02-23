using KaijuSolutions.Agents.Extensions;
using KaijuSolutions.Agents.Sensors;
using KaijuSolutions.Agents.Actuators;
using KaijuSolutions.Agents.Movement;
using UnityEngine;

//Microbe = has mthods for mating, eating, being eaten, and sensing
//Agent = movement handling (transform)
//This = the script active 


namespace KaijuSolutions.Agents.Exercises.Microbes {
    /// <summary>
    /// Basic controller for you to get started with.
    /// </summary>
    [RequireComponent(typeof(Microbe))]
    [HelpURL("https://agents.kaijusolutions.ca/manual/microbes.html#microbe-controller")]
    [AddComponentMenu("Kaiju Solutions/Agents/Exercises/Microbes/Microbe Controller", 18)]
    public class MicrobeController : KaijuController {
        /// <summary>
        /// The <see cref="Microbe"/> this is controlling.
        /// </summary>
        [Tooltip("The microbe this is controlling.")]
        [HideInInspector]
        [SerializeField]
        private Microbe microbe;
        private float lastActionTime = 0f;  // Tracker in case microbes get stuck

        /// <summary>
        /// Called after the <see cref="microbe"/> has mated.
        /// </summary>
        /// <param name="mate">The <see cref="Microbe"/> this mated with.</param>
        private void OnMate(Microbe mate) {
            
        }
        
        /// <summary>
        /// Called after the <see cref="microbe"/> has eaten.
        /// </summary>
        /// <param name="ate">The <see cref="Microbe"/> this ate.</param>
        private void OnEat(Microbe ate) {

        }

        /// <summary>
        /// Called after the <see cref="microbe"/> has been eaten.
        /// </summary>
        /// <param name="eater">The <see cref="Microbe"/> which ate this.</param>
        private void OnEaten(Microbe eater) {
            Debug.Log("I've been eaten!");
        }

        /// <summary>
        /// Called when a <see cref="MicrobeVisionSensor"/> has been run.
        /// </summary>
        /// <param name="microbeSensor">The <see cref="MicrobeVisionSensor"/> which was run.</param>
        private void OnMicrobeSensor(MicrobeVisionSensor microbeSensor) {
            if (microbeSensor.Observed.Count < 1) {
                return;
            }

            Transform nearestMicrobe = Position.Nearest(microbeSensor.Observed, out float distance);    //Log the nearest microbe and distance to it
            Microbe targetMicrobe = nearestMicrobe.GetComponent<Microbe>(); //Mark the nearest microbe as the target microbe

            if (targetMicrobe == null) {    //Check to see if the target microbe exists
                return;
            }

            if (microbe.Compatible(targetMicrobe)) { // If microbe are compatible
                if (microbe.OnCooldown == false && targetMicrobe.OnCooldown == false) { // Check if both microbes are off mating cooldown
                    Agent.Seek(nearestMicrobe, 0.25f);  //Seek
                    lastActionTime = Time.time;     //Set time of last action to now
                }
            }
            else if (microbe.Energy > targetMicrobe.Energy) {   //If im stronger
                Agent.Pursue(nearestMicrobe, 0.25f);    //Pursue! Get em!
                lastActionTime = Time.time; //Set time of last action to now
            }
            else if (microbe.Energy < targetMicrobe.Energy) {   //If im weaker
                Agent.Flee(nearestMicrobe, 1f); //RUN
                lastActionTime = Time.time; //Set time of last action to now
            }
        }

        /// <summary>
        /// Called when a <see cref="EnergyVisionSensor"/> has been run.
        /// </summary>
        /// <param name="energySensor">The <see cref="EnergyVisionSensor"/> which was run.</param>
        private void OnEnergySensor(EnergyVisionSensor energySensor) {  //When I see something on the energy sensor
            if (energySensor.Observed.Count < 1) {
                return;
            }

            Transform nearestEnergy = Position.Nearest(energySensor.Observed, out float distance);  // Mark the nearest energy pickup and distance to it
            Agent.Seek(nearestEnergy, 0.25f);   //Go get that energy
            lastActionTime = Time.time; //Set time of last action to now
        }

        private void Update() { // Custom function, fixes the freezing microbe problem
            
            if (Time.time - lastActionTime > 3f) {  //If it's been more than 3 seconds since the last action
                Agent.Wander(); //Start wandering again
                Agent.ObstacleAvoidance(clear: false);  
                lastActionTime = Time.time; //Set time of last action to now
            }
        }

        /// <summary>
        /// Callback for when a <see cref="KaijuSensor"/> has been run.
        /// </summary>
        /// <param name="sensor">The <see cref="KaijuSensor"/>.</param>
        protected override void OnSense(KaijuSensor sensor) {
            if (sensor is MicrobeVisionSensor microbeSensor) {
                OnMicrobeSensor(microbeSensor);
            }
            else if (sensor is EnergyVisionSensor energySensor) {
                OnEnergySensor(energySensor);
            }
        }

        /// <summary>
        /// Editor-only function that Unity calls when the script is loaded or a value changes in the Inspector.
        /// </summary>
        protected override void OnValidate() {
            base.OnValidate();

            if (microbe == null || microbe.transform != transform) {
                microbe = GetComponent<Microbe>();
            }
        }

        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        protected override void OnEnable() {
            if (microbe == null) {
                microbe = GetComponent<Microbe>();
                if (microbe == null) {
                    Debug.LogError("Microbe Controller - No microbe on this GameObject.", this);
                }
            }

            if (microbe != null) {
                microbe.OnMate += OnMate;
                microbe.OnEat += OnEat;
                microbe.OnEaten += OnEaten;
            }

            Agent.Wander();
            Agent.ObstacleAvoidance(clear: false);

            base.OnEnable();
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled.
        /// </summary>
        protected override void OnDisable() {
            base.OnDisable();

            if (microbe == null) {
                return;
            }

            microbe.OnMate -= OnMate;
            microbe.OnEat -= OnEat;
            microbe.OnEaten -= OnEaten;
        }
    }
}





///////////////////////////GIT COMMANDS/////////////////////////////
//git add .
//git commit -m "Fixed freezing bug and added presentation slides"
//git push
////////////////////////////////////////////////////////////////////