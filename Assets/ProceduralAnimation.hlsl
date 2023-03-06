//UNITY_SHADER_NO_UPGRADE
#ifndef MYHLSLINCLUDE_INCLUDED
#define MYHLSLINCLUDE_INCLUDED

float ProjectVectorOntoLineAsScalar(float3 vectorPosition, float3 lineOrigin, float3 lineDirection){
	float3 lineNormal = normalize(lineDirection);
	float3 relativeVectorPosition = vectorPosition - lineOrigin;
	return dot(relativeVectorPosition, lineNormal);
}

float CalculateSpline(float time, float2 firstPoint, float2 secondPoint, float2 thirdPoint, float2 fourthPoint){
	return (pow(1 - time,3) * firstPoint + 3 * time * pow(1 - time, 2) * secondPoint + 3 * pow(time, 2) * (1 - time) * thirdPoint + pow(time, 3) * fourthPoint).y;
}

float CalculatePolynomial(float first, float second, float third, float time, float offset){
	return pow(3 * (time + offset), 3) + third * pow(3 * (time + offset), 2) + second * pow(3 * (time + offset), 1) + first * pow(3 * (time + offset), 0);
}

float4 CalculateSineFactor(float amplitude, float frequency, float variable, float offset, float bias){
	return amplitude * sin(frequency * (variable + offset)) + bias;
}

float CalculateSplineWeight(float3 vertexPosition, float3 firstPoint, float3 secondPoint){
	float2 first = {0,0};
	float2 second = {0.4,0.2};
	float2 third = {0.8, 1};
	float2 fourth = {1, 4};

	float3 lineDirection = normalize(secondPoint - firstPoint);
	float3 maxDistance = ProjectVectorOntoLineAsScalar(secondPoint, firstPoint, lineDirection);
	float relativeDistanceToFirstPoint = clamp(ProjectVectorOntoLineAsScalar(vertexPosition, firstPoint, lineDirection) / maxDistance, 0, 1);

	return CalculateSpline(relativeDistanceToFirstPoint, first, second, third, fourth);
}

float CalculatPolynomialWeight(float3 vertexPosition, float3 firstPoint, float3 secondPoint, float first, float second, float third){
	float3 lineDirection = normalize(secondPoint - firstPoint);
	float3 maxDistance = ProjectVectorOntoLineAsScalar(secondPoint, firstPoint, lineDirection);
	float relativeDistanceToFirstPoint = clamp(ProjectVectorOntoLineAsScalar(vertexPosition, firstPoint, lineDirection) / maxDistance, 0, 1);
	return CalculatePolynomial(first, second, third, 0, relativeDistanceToFirstPoint);
}

float4 CalculateLineWeight(float3 vertexPosition, float3 firstPoint, float3 secondPoint, float4 firstColor, float4 secondColor){
	float3 lineDirection = normalize(firstPoint - secondPoint);
	float3 maxDistance = ProjectVectorOntoLineAsScalar(firstPoint, secondPoint, lineDirection);
	float relativeDistanceToFirstPoint = clamp(ProjectVectorOntoLineAsScalar(vertexPosition, secondPoint, lineDirection), secondPoint, firstPoint) / maxDistance;
	return firstColor * relativeDistanceToFirstPoint + secondColor * (1.0-relativeDistanceToFirstPoint);
}

float CalculateSphereWeight(float3 vertexPosition, float3 boxOrigin, float properties){
	return 0.5 - (sign(distance(vertexPosition, boxOrigin) - properties) * 0.5); // Required to minimize dynamic thread branching
}

float CalculateBoxWeight(float3 vertexPosition, float3 sphereOrigin, float2 properties){
	float distanceX = (distance(vertexPosition, sphereOrigin) - properties.x) * -1.0;
	float distanceY = (distance(vertexPosition, sphereOrigin) - properties.x) * -1.0;
	return max((sign(distanceX) * 0.5) + (sign(distanceY) * 0.5), 0); // Required to minimize dynamic thread branching
}

float4 CalculatePrimitiveWeight(float3 vertexPosition, float3 primitivePosition, float3 properties){
	if(properties.z == 1)
		return CalculateBoxWeight(vertexPosition, primitivePosition, (float) properties);

	return CalculateSphereWeight(vertexPosition, primitivePosition, (float2) properties);
}

float3 DisplacedPosition(float3 currentPosition, float3 translation, float3 rotation, float3 targetScale){
	//Rotation
	float4x4 zRotation = {
		cos(rotation.z), -sin(rotation.z), 0, 0,
		sin(rotation.z), cos(rotation.z), 0, 0,
		0, 0, 1, 0,
		0, 0, 0, 0
	};
	float4x4 yRotation = {
		cos(rotation.y), 0, sin(rotation.y), 0,
		0, 1, 0, 0,
		-sin(rotation.y), 0, cos(rotation.y), 0,
		0, 0, 0, 0
	};
	float4x4 xRotation = {
		1, 0, 0, 0,
		0, cos(rotation.x), -sin(rotation.x), 0,
		0, sin(rotation.x), cos(rotation.x), 0,
		0, 0, 0, 0
	};

	float4x4 fullRotation = mul(zRotation, mul(yRotation, xRotation));

	return (float3) mul(fullRotation, float4(currentPosition * targetScale, 0)) + translation;
}

void ProceduralShaderAnimation_float(float3 vertexPosition, float3 scale, float3 firstAxisPoint, float3 secondAxisPoint, float time, float first, float second, float third, out float3 vertexColor){
	float3 color = {0,0,0};

	float3 lineDirection = normalize(firstAxisPoint - secondAxisPoint);
	float3 maxDistance = ProjectVectorOntoLineAsScalar(firstAxisPoint, secondAxisPoint, lineDirection);
	float relativeDistanceToFirstPoint = clamp(ProjectVectorOntoLineAsScalar(vertexPosition, secondAxisPoint, lineDirection) / maxDistance, 0, 1);// / maxDistance;
	float splineWeight = CalculateSplineWeight(vertexPosition, firstAxisPoint, secondAxisPoint);
	float polynomialWeight = CalculatPolynomialWeight(vertexPosition, firstAxisPoint, secondAxisPoint, first, second, third);
	vertexPosition.y = polynomialWeight; //* 2.5f + splineWight * CalculateSineFactor(0.2, 8, time, relativeDistanceToFirstPoint, 0);
	vertexColor = vertexPosition;
	//vertexColor = ProjectVectorOntoLineAsScalar(firstAxisPoint, secondAxisPoint, lineDirection);
}
#endif //MYHLSLINCLUDE_INCLUDED