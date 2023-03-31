//UNITY_SHADER_NO_UPGRADE
#ifndef MYHLSLINCLUDE_INCLUDED
#define MYHLSLINCLUDE_INCLUDED

float ProjectVectorOntoLineAsScalar(float3 vectorPosition, float3 lineOrigin, float3 lineDirection){
	float3 lineNormal = normalize(lineDirection);
	float3 relativeVectorPosition = vectorPosition - lineOrigin;
	return dot(relativeVectorPosition, lineNormal);
}

float ClampedLineProjection(float3 vertexPosition, float3 firstPoint, float3 secondPoint){
	float3 lineDirection = normalize(secondPoint - firstPoint);
	float3 maxDistance = ProjectVectorOntoLineAsScalar(secondPoint, firstPoint, lineDirection);
	return clamp(ProjectVectorOntoLineAsScalar(vertexPosition, firstPoint, lineDirection) / maxDistance, 0, 1);
}

float PositiveLineProjection(float3 vertexPosition, float3 firstPoint, float3 secondPoint){
	float3 lineDirection = normalize(secondPoint - firstPoint);
	float3 maxDistance = ProjectVectorOntoLineAsScalar(secondPoint, firstPoint, lineDirection);
	return max(ProjectVectorOntoLineAsScalar(vertexPosition, firstPoint, lineDirection), 0);
}

float CalculateSpline(float time, float2 firstPoint, float2 secondPoint, float2 thirdPoint, float2 fourthPoint){
	return (pow(1 - time,3) * firstPoint + 3 * time * pow(1 - time, 2) * secondPoint + 3 * pow(time, 2) * (1 - time) * thirdPoint + pow(time, 3) * fourthPoint).y;
}

float CalculatePolynomial(float variable, uint2 texOffset, Texture2D animationInfo){
	uint2 texIndex = texOffset;

	float polynomialOrder = animationInfo[texIndex].x;
	texIndex.x ++;

	float offset = animationInfo[texIndex].x;
	texIndex.x ++;
	float result = offset;

	for(int currentOrder = 1; currentOrder <= polynomialOrder; currentOrder++){
		float prefix = animationInfo[texIndex].x;
		texIndex.x ++;
		result += prefix * pow(variable, currentOrder);
	}

	return result;
}

float4 CalculateSineFactor(float variable, uint2 texOffset, Texture2D animationInfo){
	uint2 texIndex = texOffset;
	float amplitude = animationInfo[texIndex].x;
	texIndex.x ++;
	float frequency = animationInfo[texIndex].x;
	texIndex.x ++;
	float bias = animationInfo[texIndex].x;

	return amplitude * sin(frequency * variable) + bias;
}

float4 CalculateSplineFactor(float variable, uint2 texOffset, Texture2D animationInfo){
	uint2 texIndex = texOffset;
	float2 firstSplinePoint = animationInfo[texIndex].xy;
	texIndex.x ++;
	float2 secondSplinePoint = animationInfo[texIndex].xy;
	texIndex.x ++;
	float2 thirdSplinePoint = animationInfo[texIndex].xy;
	texIndex.x ++;
	float2 fourthSplinePoint = animationInfo[texIndex].xy;

	float validVariable = clamp(variable, 0, 1);
	return CalculateSpline(validVariable, firstSplinePoint, secondSplinePoint, thirdSplinePoint, fourthSplinePoint);
}

float4 CalculatePolynomialFactor(float variable, uint2 texOffset, Texture2D animationInfo){
	uint2 texIndex = texOffset;
	
	float validVariable = max(variable, 0);
	return CalculatePolynomial(validVariable, texOffset, animationInfo);
}

float CalculateLineWeight(float3 vertexPosition, uint2 texOffset, Texture2D animationInfo){
	uint2 texIndex = texOffset;
	float3 firstPoint = animationInfo[texIndex].xyz;
	texIndex.x ++;
	float firstWeight = animationInfo[texIndex].x;
	texIndex.x ++;
	float3 secondPoint = animationInfo[texIndex].xyz;
	texIndex.x ++;
	float secondWeight = animationInfo[texIndex].x;

	float clampedLineDistance = ClampedLineProjection(vertexPosition, firstPoint, secondPoint);
	return firstWeight * clampedLineDistance + secondWeight * (1.0-clampedLineDistance);
}

float CalculateSplineWeight(float3 vertexPosition, uint2 texOffset, Texture2D animationInfo){
	uint2 texIndex = texOffset;
	float3 firstControlPoint = animationInfo[texIndex].xyz;
	texIndex.x ++;
	float3 secondControlPoint = animationInfo[texIndex].xyz;
	texIndex.x ++;

	float2 firstSplinePoint = animationInfo[texIndex].xy;
	texIndex.x ++;
	float2 secondSplinePoint = animationInfo[texIndex].xy;
	texIndex.x ++;
	float2 thirdSplinePoint = animationInfo[texIndex].xy;
	texIndex.x ++;
	float2 fourthSplinePoint = animationInfo[texIndex].xy;

	float clampedLineDistance = ClampedLineProjection(vertexPosition, firstControlPoint, secondControlPoint);
	return CalculateSpline(clampedLineDistance, firstSplinePoint, secondSplinePoint, thirdSplinePoint, fourthSplinePoint);
}

float CalculatPolynomialWeight(float3 vertexPosition, uint2 texOffset, Texture2D animationInfo){
	uint2 texIndex = texOffset;
	float3 firstControlPoint = animationInfo[texIndex].xyz;
	texIndex.x ++;
	float3 secondControlPoint = animationInfo[texIndex].xyz;
	texIndex.x ++;

	float lineDistance = PositiveLineProjection(vertexPosition, firstControlPoint, secondControlPoint);
	return CalculatePolynomial(lineDistance, texIndex, animationInfo);
}

float CalculateSphereWeight(float3 vertexPosition, float3 boxOrigin, float radius){
	return 0.5 - (sign(distance(vertexPosition, boxOrigin) - radius) * 0.5); // Required to minimize dynamic thread branching !!Vertices on the edge are half weighted!!
}

float CalculateBoxWeight(float3 vertexPosition, float3 sphereOrigin, float3 properties){
	float distanceX = (distance(vertexPosition.x, sphereOrigin.x) - properties.x) * -1.0;
	float distanceY = (distance(vertexPosition.y, sphereOrigin.y) - properties.y) * -1.0;
	float distanceZ = (distance(vertexPosition.z, sphereOrigin.z) - properties.z) * -1.0;
	return max(sign(distanceX) + sign(distanceY) + sign(distanceZ) - 2, 0); // Required to minimize dynamic thread branching !!Vertices on the edge are half weighted!!
}

float4 CalculatePrimitiveWeight(float3 vertexPosition, uint2 texOffset, uint type, Texture2D animationInfo){
	uint2 texIndex = texOffset;
	float3 origin = animationInfo[texIndex].xyz;
	texIndex.x ++;
	float3 dimensions = animationInfo[texIndex].xyz;

	if(type == 4) return CalculateSphereWeight(vertexPosition, origin, (float) dimensions.x);

	return CalculateBoxWeight(vertexPosition, origin, dimensions);
}

float CalculateWeigth(float3 vertexPosition, uint weightCount, uint2 texOffset, Texture2D animationInfo){
	uint2 texIndex = texOffset;
	float amountedWeight = 1;

	for(uint weightIndex = 0; weightIndex < weightCount; weightIndex++){
		uint type = (uint) animationInfo[texIndex].x;
		texIndex.x++;

		if(type == 1){
			amountedWeight *= CalculateLineWeight(vertexPosition, texIndex, animationInfo);
		} 
		else if(type == 2){
			amountedWeight *= CalculateSplineWeight(vertexPosition, texIndex, animationInfo);
		}
		else if(type == 3){
			amountedWeight *= CalculatPolynomialWeight(vertexPosition, texIndex, animationInfo);
		}
		else if(type == 4 || type == 5){
			amountedWeight *= CalculatePrimitiveWeight(vertexPosition, texIndex, type, animationInfo);
		}

		texIndex.y ++;
		texIndex.x = texOffset.x;
	}
	return amountedWeight;
}

float CalculateInfluence(float3 vertexPosition, float time, float offset, float duration, uint influenceCount, uint2 texOffset, Texture2D animationInfo){
	uint2 texIndex = texOffset;
	float amountedInfluence = 0;

	for(uint influenceIndex = 0; influenceIndex < influenceCount; influenceIndex++){
		uint type = (uint) animationInfo[texIndex].x;
		texIndex.x++;
		float2 variableParticipation = animationInfo[texIndex].xy;
		texIndex.x++;
		float variable = time * variableParticipation.x + offset * variableParticipation.y;
		float scaledVariable = variable;

		if(type == 1){
			amountedInfluence += CalculateSineFactor(variable, texIndex, animationInfo);
		} 
		else if(type == 2){
			if(duration != 0) scaledVariable = time / duration * variableParticipation.x + offset * variableParticipation.y;
			amountedInfluence += CalculateSplineFactor(scaledVariable, texIndex, animationInfo);
		}
		else if(type == 3){
			amountedInfluence += CalculatePolynomialFactor(variable, texIndex, animationInfo);
		}

		texIndex.y ++;
		texIndex.x = texOffset.x;
	}

	return amountedInfluence;
}

float3 DisplacedPosition(float3 currentPosition, float boundingScale, float3 translation, float3 rotation, float3 scale){
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

	return (float3) mul(fullRotation, float4(currentPosition * scale, 0)) + (translation * boundingScale);
}

void ProceduralShaderAnimation_float(float3 vertexPosition, float3 boundingOrigin, float boundingScale, float time, Texture2D animationInfo,  out float3 displacedVertexPosition){
	float3 targetTranslation = {0,0,0};
	float3 targetRotation 	 = {0,0,0};
	float3 targetScale 		 = {1,1,1};

	float validatedTime = time;

	float3 scaledVertexPosition = (vertexPosition + boundingScale - boundingOrigin) / boundingScale;

	float3 origin = {1, 1, 1};

	uint2 texIndex = {0, 0};

	float animationLength = animationInfo[texIndex].x;
	texIndex.x ++;
	uint contentLength = (uint) animationInfo[texIndex].x;

	if(animationLength != 0) validatedTime = validatedTime % animationLength;

	texIndex.y ++;
	texIndex.x = 0;

	while(texIndex.y < contentLength){
		uint transformationType = (uint) animationInfo[texIndex].x;
		texIndex.x ++;

		float3 translationAxis = normalize(animationInfo[texIndex].xyz);
		texIndex.x++;

		float3 offsetAxis = animationInfo[texIndex].xyz;
		float3 currentOrigin = origin - offsetAxis;
		texIndex.x ++;

		uint weightCount = (uint) animationInfo[texIndex].x;
		texIndex.x ++;

		uint influenceCount = (uint) animationInfo[texIndex].x;
		texIndex.y ++;
		texIndex.x = 0;

		float offset = ProjectVectorOntoLineAsScalar(scaledVertexPosition, currentOrigin, offsetAxis) / 2;

		float weight = CalculateWeigth(scaledVertexPosition, weightCount, texIndex, animationInfo);
		texIndex.y += weightCount;
		float influence = CalculateInfluence(scaledVertexPosition, validatedTime, offset, animationLength, influenceCount, texIndex, animationInfo);
		float weightedInfluence = weight * influence;

		if(transformationType == 1){
			targetTranslation += translationAxis * weightedInfluence;
		} 
		else if(transformationType == 2){
			targetRotation += translationAxis * weightedInfluence;
		}
		else if(transformationType == 3){
			targetScale += translationAxis * weightedInfluence;
		}

		texIndex.y += influenceCount;
		texIndex.x = 0;
	}

	float3 targetPosition = DisplacedPosition(vertexPosition, boundingScale, targetTranslation, targetRotation, targetScale);
	displacedVertexPosition = targetPosition;
}

void DebugInfo_float(float3 vertexPosition, float3 boundingOrigin, float boundingScale, float time, float groupOffset, Texture2D animationInfo, out float3 displacedVertexPosition){

	float3 scaledVertexPosition = (vertexPosition + boundingScale - boundingOrigin) / boundingScale;

	float3 origin = {1, 1, 1};

	uint2 texIndex = {0, 0};

	float animationLength = animationInfo[texIndex].x;
	texIndex.x ++;
	uint contentLength = (uint) animationInfo[texIndex].x;

	texIndex.y ++;
	texIndex.x = 0;
	texIndex.y += groupOffset;

	uint transformationType = (uint) animationInfo[texIndex].x;
	texIndex.x ++;

	float3 translationAxis = normalize(animationInfo[texIndex].xyz);
	texIndex.x++;

	float3 offsetAxis = animationInfo[texIndex].xyz;
	float3 currentOrigin = origin - offsetAxis;
	texIndex.x ++;

	uint weightCount = (uint) animationInfo[texIndex].x;
	texIndex.x ++;

	uint influenceCount = (uint) animationInfo[texIndex].x;
	texIndex.y ++;
	texIndex.x = 0;

	float offset = ProjectVectorOntoLineAsScalar(scaledVertexPosition, currentOrigin, offsetAxis) / 2;

	float weight = CalculateWeigth(scaledVertexPosition, weightCount, texIndex, animationInfo);

	displacedVertexPosition = weight;
}
#endif //MYHLSLINCLUDE_INCLUDED