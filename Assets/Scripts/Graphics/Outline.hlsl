TEXTURE2D(_CameraColorTexture);
SAMPLER(sampler_CameraColorTexture);
float4 _CameraColorTexture_TexelSize;

TEXTURE2D(_CameraDepthTexture);
SAMPLER(sampler_CameraDepthTexture);
float4 _CameraDepthTexture_TexelSize;

TEXTURE2D(_CameraDepthNormalsTexture);
SAMPLER(sampler_CameraDepthNormalsTexture);
 

float3 DecodeNormal(float4 enc) {
    float kScale = 1.7777;
    float3 nn = enc.xyz*float3(2*kScale,2*kScale,0) + float3(-kScale,-kScale,1);
    float g = 2.0 / dot(nn.xyz,nn.xyz);
    float3 n;
    n.xy = g*nn.xy;
    n.z = g-1;
    return n;
}

void OutlineObject_float(float2 UV, float3 CameraDir, float3 ObjNormal, float OutlineThickness, float DepthSensitivity, float NormalsSensitivity, float DepthCheck, out float Out) {
	float halfScaleFloor = floor(OutlineThickness * 0.5);
	float halfScaleCeil = ceil(OutlineThickness * 0.5);

	float2 uvSamples[4];
	float depthSamples[4];
	float3 normalSamples[4];

	uvSamples[0] = UV - float2(_CameraDepthTexture_TexelSize.x, _CameraDepthTexture_TexelSize.y) * halfScaleFloor;
	uvSamples[1] = UV + float2(_CameraDepthTexture_TexelSize.x, _CameraDepthTexture_TexelSize.y) * halfScaleCeil;
	uvSamples[2] = UV + float2(_CameraDepthTexture_TexelSize.x * halfScaleCeil, -_CameraDepthTexture_TexelSize.y * halfScaleFloor);
	uvSamples[3] = UV + float2(-_CameraDepthTexture_TexelSize.x * halfScaleFloor, _CameraDepthTexture_TexelSize.y * halfScaleCeil);

	for (int i = 0; i < 4; i++) {
		depthSamples[i] = SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, uvSamples[i]).r;
		normalSamples[i] = DecodeNormal(SAMPLE_TEXTURE2D(_CameraDepthNormalsTexture, sampler_CameraDepthNormalsTexture, uvSamples[i]));
	}

	float depthCur = SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, UV).r;
	float3 normalCur;
	normalCur = normalize(DecodeNormal(SAMPLE_TEXTURE2D(_CameraDepthNormalsTexture, sampler_CameraDepthNormalsTexture, UV)));

	// Normals
	float3 normalFiniteDifference0 = normalSamples[1] - normalSamples[0];
	float3 normalFiniteDifference1 = normalSamples[3] - normalSamples[2];
	float edgeNormal = sqrt(dot(normalFiniteDifference0, normalFiniteDifference0) + dot(normalFiniteDifference1, normalFiniteDifference1));
	edgeNormal = edgeNormal > (1 / NormalsSensitivity) ? 1 : 0;

	// Depth Sensitivity Adjusted Based On Normal VS Camera Difference
	float cameraSurfaceAngle = abs(abs(acos(dot(ObjNormal, CameraDir)) - (PI / 2)) - (PI / 2));
	float cameraDiff = cameraSurfaceAngle * 2 / PI * 100;

	// Depth
	float edgeDepth;
	if (DepthCheck != 0) {
		float depthFiniteDifference0 = depthSamples[1] - depthSamples[0];
		float depthFiniteDifference1 = depthSamples[3] - depthSamples[2];
		edgeDepth = sqrt(pow(depthFiniteDifference0, 2) + pow(depthFiniteDifference1, 2)) * 100;

		if (cameraSurfaceAngle > (PI / 2 - 0.5)) {
			edgeDepth = 0;
		} else if (cameraSurfaceAngle < 0.5) {
			edgeDepth = edgeDepth > 0.001 ? 1 : 0;
		} else {
			float depthThreshold = (1 / (cameraDiff / DepthSensitivity)) * depthSamples[0];
			edgeDepth = edgeDepth > depthThreshold ? 1 : 0;
		}
	} else {
		edgeDepth = 0;
	}

    float edge = max(edgeDepth, edgeNormal);
    Out = edge;
}