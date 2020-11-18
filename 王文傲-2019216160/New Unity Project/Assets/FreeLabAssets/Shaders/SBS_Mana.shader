// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "SBS/Mana" {

	Properties 
	{
		_MainTex1 ("Base1 (RGB)", 2D) = "white" {}
		_Color1("Color1", Color) = (1,1,1,1)
      	_XScrollSpeed1 ( "X Scroll Speed 1", Float ) = 1
      	_YScrollSpeed1 ( "Y Scroll Speed 1", Float ) = 1

		_MainTex2 ("Base2 (RGB)", 2D) = "white" {}
		_Color2 ("Color2", Color) = (1.0, 1.0, 1.0, 1.0)
      	_XScrollSpeed2 ( "X Scroll Speed 2", Float ) = 1
      	_YScrollSpeed2 ( "Y Scroll Speed 2", Float ) = 1

		_MainTex3 ("Base3 (RGB)", 2D) = "white" {}
		_RotationSpeed  ( "Rotation Speed", Float ) = 1
		_Color3 ("Color3", Color) = (1.0, 1.0, 1.0, 1.0)

		_MainTex4 ("Base4 (RGB)", 2D) = "white" {}
      	_XScrollSpeed3 ( "X Scroll Speed 3", Float ) = 1
      	_YScrollSpeed3 ( "Y Scroll Speed 3", Float ) = 1
      	_Height ("Cutoff Height", Range(0,1)) = 1.0
	}

   SubShader {
 
      Pass {
         Cull Front 
 
         CGPROGRAM 
 
         #pragma vertex vert  
         #pragma fragment frag
		 
		#include "UnityCG.cginc"
        #include "AutoLight.cginc"

		uniform sampler2D  _MainTex1;
		uniform half4      _MainTex1_ST;

		uniform sampler2D _MainTex2;
		uniform half4      _MainTex2_ST;

		uniform sampler2D  _MainTex3;
		uniform half4      _MainTex3_ST;

		uniform sampler2D  _MainTex4;
		uniform half4      _MainTex4_ST;

	    half	 _XScrollSpeed1;
	    half	 _YScrollSpeed1;

	    half	 _XScrollSpeed2;
	    half	 _YScrollSpeed2;

	    half	 _XScrollSpeed3;
	    half	 _YScrollSpeed3;
	      	
	    half	  _Height;

		uniform half4 _Color1;
		uniform half4 _Color2;
		uniform half4 _Color3;

		uniform half _RotationSpeed;
 
         struct vertexInput 
		 {
            half4 vertex : POSITION;
         };

         struct vertexOutput 
		 {
            half4		pos : SV_POSITION;
            half4		posInObjectCoords : TEXCOORD0;
			half4		uv1 : TEXCOORD1;
			half4		uv2 : TEXCOORD2;
         };
 
         vertexOutput vert(appdata_tan v) 
         {
            vertexOutput o;
 
            o.pos =  UnityObjectToClipPos(v.vertex);
            o.posInObjectCoords = v.vertex; 
 
  			float ltime    = _Time.x;
			half2 scrollUV = v.texcoord;						    
			half  xScrollValue = _XScrollSpeed1 * ltime;
			half  yScrollValue = _YScrollSpeed1 * ltime;
			scrollUV += half2( xScrollValue, yScrollValue );
				
			o.uv1.xy = TRANSFORM_TEX(scrollUV, _MainTex1);

			scrollUV = v.texcoord;
			xScrollValue = _XScrollSpeed2 * ltime;
			yScrollValue = _YScrollSpeed2 * ltime;
			scrollUV += half2( xScrollValue, yScrollValue );

			o.uv1.zw = TRANSFORM_TEX(scrollUV, _MainTex2);

			float sinX = sin( _RotationSpeed * ltime );
			float cosX = cos( _RotationSpeed * ltime );

			float2x2 rotationMatrix = float2x2( cosX, -sinX, sinX, cosX );

			o.uv2.xy = mul( v.texcoord.xy, rotationMatrix) * _MainTex3_ST.xy + _MainTex3_ST.zw;

			scrollUV = v.texcoord;	
			xScrollValue = _XScrollSpeed3 * ltime;
			yScrollValue = _YScrollSpeed3 * ltime;

			scrollUV += half2( xScrollValue, yScrollValue );

			o.uv2.zw = TRANSFORM_TEX(scrollUV, _MainTex3);

            return o;
         }
 
         half4 frag(vertexOutput i) : COLOR 
         {
			 /*
            if (i.posInObjectCoords.y > _Height) 
            {
               discard; 
            }*/

            half4 tex1 = tex2D( _MainTex1, i.uv1.xy );
			half4 tex2 = tex2D( _MainTex2, i.uv1.zw );
			half4 tex3 = tex2D( _MainTex3, i.uv2.xy );
			half4 tex4 = tex2D( _MainTex4, i.uv2.zw );

			half3 tex1Final  = ((tex1 * 10.0) * tex2) * _Color1;

			half3 FinalColor = tex3 * _Color2 * (tex1Final * 2)  + tex1Final * 0.5 + (tex4 * _Color3);
				
			return half4( tex1Final.rgb * tex4.rgb * (_Color1.rgb * _Color2.rgb), 1.0);
         }
 
         ENDCG  
      }
 
      Pass {
         Cull Back 
 
         CGPROGRAM 
 
         #pragma vertex vert  
         #pragma fragment frag 

		#include "UnityCG.cginc"
        #include "AutoLight.cginc"

		uniform sampler2D  _MainTex1;
		uniform half4      _MainTex1_ST;

		uniform sampler2D _MainTex2;
		uniform half4      _MainTex2_ST;

		uniform sampler2D  _MainTex3;
		uniform half4      _MainTex3_ST;

		uniform sampler2D  _MainTex4;
		uniform half4      _MainTex4_ST;

	    half	 _XScrollSpeed1;
	    half	 _YScrollSpeed1;

	    half	 _XScrollSpeed2;
	    half	 _YScrollSpeed2;

	    half	 _XScrollSpeed3;
	    half	 _YScrollSpeed3;
	      	
	    half	  _Height;

		uniform half4 _Color1;
		uniform half4 _Color2;
		uniform half4 _Color3;

		uniform half _RotationSpeed;	

         struct vertexInput 
		 {
            half4 vertex : POSITION;
         };

         struct vertexOutput 
		 {
            half4		pos : SV_POSITION;
            half4		posInObjectCoords : TEXCOORD0;
			half4		uv1 : TEXCOORD1;
			half4		uv2 : TEXCOORD2;
         };
 
         vertexOutput vert(appdata_tan v) 
         {
            vertexOutput o;
 
            o.pos =  UnityObjectToClipPos(v.vertex);
            o.posInObjectCoords = v.vertex; 
 
 			half  ltime    = _Time.x;
			half2 scrollUV = v.texcoord;						    
			half  xScrollValue = _XScrollSpeed1 * ltime;
			half  yScrollValue = _YScrollSpeed1 * ltime;
			scrollUV += half2( xScrollValue, yScrollValue );
				
			o.uv1.xy = TRANSFORM_TEX(scrollUV, _MainTex1);

			scrollUV = v.texcoord;
			xScrollValue = _XScrollSpeed2 * ltime;
			yScrollValue = _YScrollSpeed2 * ltime;
			scrollUV += half2( xScrollValue, yScrollValue );

			o.uv1.zw = TRANSFORM_TEX(scrollUV, _MainTex2);

			float sinX = sin ( _RotationSpeed * ltime );
			float cosX = cos ( _RotationSpeed * ltime );

			float2x2 rotationMatrix = float2x2( cosX, -sinX, sinX, cosX );

			o.uv2.xy = mul( v.texcoord.xy, rotationMatrix) * _MainTex3_ST.xy + _MainTex3_ST.zw;

			scrollUV = v.texcoord;	
			xScrollValue = _XScrollSpeed3 * ltime;
			yScrollValue = _YScrollSpeed3 * ltime;

			scrollUV += half2( xScrollValue, yScrollValue );

			o.uv2.zw = TRANSFORM_TEX(scrollUV, _MainTex3);

            return o;
         }
 
         half4 frag(vertexOutput i) : COLOR 
         {
			 /*
            if (i.posInObjectCoords.y > _Height) 
            {
               discard; 
            }
			*/

			half4 tex1 = tex2D( _MainTex1, i.uv1.xy );
			half4 tex2 = tex2D( _MainTex2, i.uv1.zw );
			half4 tex3 = tex2D( _MainTex3, i.uv2.xy );
			half4 tex4 = tex2D( _MainTex4, i.uv2.zw );

			half3 tex1Final  = ((tex1 * 10.0) * tex2) * _Color1;

			half3 FinalColor = tex3 * _Color2 * (tex1Final * 2)  + tex1Final * 0.5 + (tex4 * _Color3);
				
			return half4( FinalColor.rgb, 1.0);
         }
 
         ENDCG  
      }
   }
}