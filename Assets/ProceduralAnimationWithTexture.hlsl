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
	return max(ProjectVectorOntoLineAsScalar(vertexPosition, firstPoint, lineDirection) / maxDistance, 0);
}

float CalculateSpline(float time, float2 firstPoint, float2 secondPoint, float2 thirdPoint, float2 fourthPoint){
	return (pow(1 - time,3) * firstPoint + 3 * time * pow(1 - time, 2) * secondPoint + 3 * pow(time, 2) * (1 - time) * thirdPoint + pow(time, 3) * fourthPoint).y;
}

float CalculatePolynomial(float variable, uint row, uint column, Texture2D animationInfo){
	uint2 texIndex = {row, column};

	float polynomialOrder = animationInfo[texIndex].x;
	texIndex.y ++;

	float offset = animationInfo[texIndex].x;
	texIndex.y ++;
	float result = offset;

	for(int currentOrder = 1; currentOrder <= polynomialOrder; currentOrder++){
		float prefix = animationInfo[texIndex].x;
		texIndex.y ++;
		result += prefix * pow(variable, currentOrder);
	}

	return result;
}

float4 CalculateSineFactor(float variable, uint row, Texture2D animationInfo){
	uint2 texIndex = {row, 1};
	float amplitude = animationInfo[texIndex].x;
	texIndex.y ++;
	float frequency = animationInfo[texIndex].x;
	texIndex.y ++;
	float timeUsed = animationInfo[texIndex].x;
	texIndex.y ++;
	float offsetUsed = animationInfo[texIndex].x;
	texIndex.y ++;
	float bias = animationInfo[texIndex].x;

	return amplitude * sin(frequency * variable) + bias;
}

float4 CalculateSplineFactor(float variable, uint row, Texture2D animationInfo){
	uint2 texIndex = {row, 1};
	float2 firstSplinePoint = animationInfo[texIndex].xy;
	texIndex.y ++;
	float2 secondSplinePoint = animationInfo[texIndex].xy;
	texIndex.y ++;
	float2 thirdSplinePoint = animationInfo[texIndex].xy;
	texIndex.y ++;
	float2 fourthSplinePoint = animationInfo[texIndex].xy;

	float validVariable = clamp(variable, 0, 1);
	return CalculateSpline(validVariable, firstSplinePoint, secondSplinePoint, thirdSplinePoint, fourthSplinePoint);
}

float4 CalculatePolynomialFactor(float variable, uint row, Texture2D animationInfo){
	uint2 texIndex = {row, 1};
	
	float validVariable = max(variable, 0);
	return CalculatePolynomial(validVariable, texIndex.x, texIndex.y, animationInfo);
}

float CalculateSplineWeight(float3 vertexPosition, uint row, Texture2D animationInfo){
	uint2 texIndex = {row, 1};
	float3 firstControlPoint = animationInfo[texIndex].xyz;
	texIndex.y ++;
	float3 secondControlPoint = animationInfo[texIndex].xyz;
	texIndex.y ++;

	float2 firstSplinePoint = animationInfo[texIndex].xy;
	texIndex.y ++;
	float2 secondSplinePoint = animationInfo[texIndex].xy;
	texIndex.y ++;
	float2 thirdSplinePoint = animationInfo[texIndex].xy;
	texIndex.y ++;
	float2 fourthSplinePoint = animationInfo[texIndex].xy;

	float clampedLineDistance = ClampedLineProjection(vertexPosition, firstControlPoint, secondControlPoint);
	return CalculateSpline(clampedLineDistance, firstSplinePoint, secondSplinePoint, thirdSplinePoint, fourthSplinePoint);
}

float CalculatPolynomialWeight(float3 vertexPosition, uint row, Texture2D animationInfo){
	uint2 texIndex = {row, 1};
	float3 firstControlPoint = animationInfo[texIndex].xyz;
	texIndex.y ++;
	float3 secondControlPoint = animationInfo[texIndex].xyz;
	texIndex.y ++;

	float lineDistance = PositiveLineProjection(vertexPosition, firstControlPoint, secondControlPoint);
	return CalculatePolynomial(lineDistance, texIndex.x, texIndex.y, animationInfo);
}

float4 CalculateLineWeight(float3 vertexPosition, uint row, Texture2D animationInfo){
	uint2 texIndex = {row, 1};
	float3 firstPoint = animationInfo[texIndex].xyz;
	texIndex.y ++;
	float firstWeight = animationInfo[texIndex].x;
	texIndex.y ++;
	float3 secondPoint = animationInfo[texIndex].xyz;
	texIndex.y ++;
	float secondWeight = animationInfo[texIndex].x;

	float clampedLineDistance = ClampedLineProjection(vertexPosition, firstPoint, secondPoint);
	return firstWeight * clampedLineDistance + secondWeight * (1.0-clampedLineDistance);
}

float CalculateSphereWeight(float3 vertexPosition, float3 boxOrigin, float properties){
	return 0.5 - (sign(distance(vertexPosition, boxOrigin) - properties) * 0.5); // Required to minimize dynamic thread branching
}

float CalculateBoxWeight(float3 vertexPosition, float3 sphereOrigin, float2 properties){
	float distanceX = (distance(vertexPosition, sphereOrigin) - properties.x) * -1.0;
	float distanceY = (distance(vertexPosition, sphereOrigin) - properties.x) * -1.0;
	return max((sign(distanceX) * 0.5) + (sign(distanceY) * 0.5), 0); // Required to minimize dynamic thread branching
}

float4 CalculatePrimitiveWeight(float3 vertexPosition, uint row, Texture2D animationInfo){
	uint2 texIndex = {row, 3};
	uint type = (uint) animationInfo[texIndex].x;
	texIndex.y ++;
	float3 origin = animationInfo[texIndex].xyz;
	texIndex.y ++;
	float3 dimensions = animationInfo[texIndex].xyz;

	if(type == 1)
		return CalculateBoxWeight(vertexPosition, origin, (float2) dimensions.xy);

	return CalculateSphereWeight(vertexPosition, origin, (float) dimensions.x);
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

float CalculateWeigth(float3 vertexPosition, uint weightCount, uint rowOffset, Texture2D animationInfo){
	uint2 texIndex = {rowOffset, 2};
	float amountedWeight = 0;

	for(uint weightIndex = 0; weightIndex < weightCount; weightIndex++){
		if(animationInfo[texIndex].x == 1){
			amountedWeight += CalculateLineWeight(vertexPosition, texIndex.x, animationInfo);
		} 
		else if(animationInfo[texIndex].x == 2){
			amountedWeight += CalculatePrimitiveWeight(vertexPosition, texIndex.x, animationInfo);
		}
		else if(animationInfo[texIndex].x == 3){
			amountedWeight += CalculatPolynomialWeight(vertexPosition, texIndex.x, animationInfo);
		}
		else if(animationInfo[texIndex].x == 4){
			amountedWeight += CalculateSplineWeight(vertexPosition, texIndex.x, animationInfo);
		}

		texIndex.x ++;
	}
	return amountedWeight;
}

float CalculateInfluence(float3 vertexPosition, float time, float offset, uint influenceCount, uint2 texOffset, Texture2D animationInfo){
	uint2 texIndex = texOffset;
	float amountedInfluence = 0;

	for(uint influenceIndex = 0; influenceIndex < influenceCount; influenceIndex++){
		uint type = (uint) animationInfo[texIndex].x;
		texIndex.y++;
		float2 variableParticipation = animationInfo[texIndex].xy;
		texIndex.y++;
		float variable = time * variableParticipation.x + offset * variableParticipation.y;

		if(animationInfo[texIndex].x == 1){
			amountedInfluence += CalculateSineFactor(variable, texIndex.x, animationInfo);
		} 
		else if(animationInfo[texIndex].x == 2){
			amountedInfluence += CalculateSplineFactor(variable, texIndex.x, animationInfo);
		}
		else if(animationInfo[texIndex].x == 3){
			amountedInfluence += CalculatePolynomialFactor(variable, texIndex.x, animationInfo);
		}

		texIndex.x ++;
	}

	return amountedInfluence;
}


float3 ProceduralShaderAnimation(float3 vertexPosition, float3 objectScale, float time, Texture2D animationInfo){
	float3 targetTranslation = {0,0,0};
	float3 targetRotation 	 = {0,0,0};
	float3 targetScale 		 = {1,1,1};

	float3 origin = {0, 0, 0};

	uint2 texIndex = {0, 0};

	float animationLength = animationInfo[texIndex].x;
	texIndex.y ++;
	uint contentLength = (uint) animationInfo[texIndex].x;

	texIndex.x ++;
	texIndex.y = 0;

	while(texIndex.x < contentLength){
		uint transformationType = (uint) animationInfo[texIndex].x;
		texIndex.y ++;
		float3 axis = animationInfo[texIndex].xyz;
		texIndex.y ++;
		uint weightCount = (uint) animationInfo[texIndex].x;
		texIndex.y ++;
		uint influenceCount = (uint) animationInfo[texIndex].x;

		float offset = ProjectVectorOntoLineAsScalar(vertexPosition, origin, axis);

		float weight = CalculateWeigth(vertexPosition, weightCount, texIndex.x, animationInfo);
		float influence = CalculateInfluence(vertexPosition, time, offset, influenceCount, texIndex.x +  weightCount, animationInfo);

		float weightedInfluence = weight * influence;

		if(transformationType == 1){
			targetTranslation += axis * weightedInfluence;
		} 
		else if(transformationType == 2){
			targetRotation += axis * weightedInfluence;
		}
		else if(transformationType == 3){
			targetRotation += axis * weightedInfluence;
		}

		texIndex.x += weightCount + influenceCount;
		texIndex.y = 0;
	}

	float3 targetPosition = DisplacedPosition(vertexPosition, targetTranslation, targetRotation, targetScale);

	return targetPosition - vertexPosition;
}
#endif //MYHLSLINCLUDE_INCLUDED