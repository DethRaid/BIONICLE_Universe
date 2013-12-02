public class EntityStats {

	var entityID;
	
	var physicalStrength : int;
	var physicalResistance : int;
	var healthPoints : int;
	
	var essenceStrength : int;
	var essenceResistance : int;
	var essencePoints : int;
	
	var hitChance : int;
	var dodgeChance : int;
	var speed : int;

	var strength : int = physicalStrength + physicalResistance + healthPoints;
	var essence : int = essenceStrength + essenceResistance + essencePoints;
	var agility : int = hitChance + dodgeChance + speed;
	
	var basePrefab;
	
	var entityType;
	
	var aiState : int;
	
	var spawnVector : Vector3;
	
	var mask : int;
	var weapon : int;
}