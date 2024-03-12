extends Node2D

var input_list = []
var time = []
const ALPHANUM = [
	'a','b','c','d','e','f','g','h','i','j','k','l','m',
	'n','o','p','q','r','s','t','u','v','w','x','y','z',
	'0','1','2','3','4','5','6','7','8','9'
	]

var current_mean
var lowest = 1.0
var highest = 0.0
func _process(_delta):
	var start = Time.get_unix_time_from_system()
	for a in ALPHANUM:
		if Input.is_action_pressed(a):
			input_list.append(a)
	var end = Time.get_unix_time_from_system()
	
	time.append(end - start)
	$name.text = "GDScript output. " + str(time.size())
	current_mean = mean(time)
	if time.size() > 1000:
		time.pop_front()
		if current_mean < lowest:
			$lowest.text = "Lowest mean time: " + str(current_mean)
			lowest = current_mean
		elif current_mean > highest:
			$highest.text = "Highest mean time: " + str(current_mean)
			highest = current_mean
	
	$"output".text = " , ".join(input_list)
	input_list.clear()

func mean(arr):
	var total = float()
	for a in arr:
		total += a
	return total / float(arr.size())
