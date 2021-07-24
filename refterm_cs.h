#if 0
//
// Generated by Microsoft (R) D3D Shader Disassembler
//
//
// Input signature:
//
// Name                 Index   Mask Register SysValue  Format   Used
// -------------------- ----- ------ -------- -------- ------- ------
// no Input
//
// Output signature:
//
// Name                 Index   Mask Register SysValue  Format   Used
// -------------------- ----- ------ -------- -------- ------- ------
// no Output
// Debug name: 2c0172aaff8bf4abbe59825a1b722cbf.pdb
cs_5_0
dcl_globalFlags refactoringAllowed | skipOptimization
dcl_constantbuffer CB0[3], immediateIndexed
dcl_resource_structured t0, 12
dcl_resource_texture2d (float,float,float,float) t1
dcl_uav_typed_texture2d (float,float,float,float) u0
dcl_input vThreadID.xy
dcl_temps 6
dcl_thread_group 8, 8, 1
mov r0.xyzw, vThreadID.xyyy
nop 
mov r0.xy, r0.xyxx
ineg r1.xy, cb0[1].xyxx
iadd r1.xy, r0.xyxx, r1.xyxx
udiv r1.xy, null, r1.xyxx, cb0[0].xyxx
ineg r1.zw, cb0[1].xxxy
iadd r1.zw, r0.xxxy, r1.zzzw
udiv null, r1.zw, r1.zzzw, cb0[0].xxxy
uge r2.x, r0.x, cb0[1].x
uge r2.y, r0.y, cb0[1].y
and r2.x, r2.y, r2.x
ult r2.y, r1.x, cb0[0].z
and r2.x, r2.y, r2.x
ult r2.y, r1.y, cb0[0].w
and r2.x, r2.y, r2.x
if_nz r2.x
  mov r2.x, cb0[0].z
  imul null, r1.y, r1.y, r2.x
  iadd r1.x, r1.x, r1.y
  ld_structured_indexable(structured_buffer, stride=12)(mixed,mixed,mixed,mixed) r1.y, r1.x, l(0), t0.xxxx
  ld_structured_indexable(structured_buffer, stride=12)(mixed,mixed,mixed,mixed) r2.x, r1.x, l(4), t0.xxxx
  ld_structured_indexable(structured_buffer, stride=12)(mixed,mixed,mixed,mixed) r1.x, r1.x, l(8), t0.xxxx
  nop 
  mov r1.y, r1.y
  and r3.x, r1.y, l(0x0000ffff)
  mov r2.y, l(16)
  ushr r3.y, r1.y, r2.y
  mov r3.x, r3.x
  mov r3.y, r3.y
  imul null, r2.yz, r3.xxyx, cb0[0].xxyx
  iadd r3.xy, r1.zwzz, r2.yzyy
  mov r3.zw, l(0,0,0,0)
  ld_indexable(texture2d)(float,float,float,float) r3.xyzw, r3.xyzw, t1.xyzw
  nop 
  mov r1.x, r1.x
  and r1.y, r1.x, l(255)
  mov r1.z, l(8)
  ushr r1.z, r1.x, r1.z
  and r1.z, r1.z, l(255)
  mov r2.y, l(16)
  ushr r1.x, r1.x, r2.y
  and r1.x, r1.x, l(255)
  itof r4.x, r1.y
  itof r4.y, r1.z
  itof r4.z, r1.x
  div r1.xyz, r4.xyzx, l(255.000000, 255.000000, 255.000000, 0.000000)
  mov r1.xyz, r1.xyzx
  nop 
  mov r2.x, r2.x
  and r2.y, r2.x, l(255)
  mov r2.z, l(8)
  ushr r2.z, r2.x, r2.z
  and r2.z, r2.z, l(255)
  mov r2.w, l(16)
  ushr r2.w, r2.x, r2.w
  and r2.w, r2.w, l(255)
  itof r4.x, r2.y
  itof r4.y, r2.z
  itof r4.z, r2.w
  div r2.yzw, r4.xxyz, l(0.000000, 255.000000, 255.000000, 255.000000)
  mov r2.yzw, r2.yyzw
  nop 
  mov r4.x, cb0[1].z
  and r4.y, r4.x, l(255)
  mov r4.z, l(8)
  ushr r4.z, r4.x, r4.z
  and r4.z, r4.z, l(255)
  mov r4.w, l(16)
  ushr r4.x, r4.x, r4.w
  and r4.x, r4.x, l(255)
  itof r5.x, r4.y
  itof r5.y, r4.z
  itof r5.z, r4.x
  div r4.xyz, r5.xyzx, l(255.000000, 255.000000, 255.000000, 0.000000)
  mov r4.xyz, r4.xyzx
  mov r4.w, l(28)
  ushr r4.w, r2.x, r4.w
  mov r5.x, l(1)
  and r4.w, r4.w, r5.x
  ine r4.w, l(0, 0, 0, 0), r4.w
  if_nz r4.w
    mul r2.yzw, r2.yyzw, r4.xxyz
  endif 
  mov r4.x, l(25)
  ushr r4.x, r2.x, r4.x
  mov r4.y, l(1)
  and r4.x, r4.y, r4.x
  ine r4.x, l(0, 0, 0, 0), r4.x
  if_nz r4.x
    mul r2.yzw, r2.yyzw, l(0.000000, 0.500000, 0.500000, 0.500000)
  endif 
  itof r4.x, l(1)
  mov r3.w, -r3.w
  add r3.w, r3.w, r4.x
  mul r1.xyz, r1.xyzx, r3.wwww
  mul r3.xyz, r2.yzwy, r3.xyzx
  add r3.xyz, r1.xyzx, r3.xyzx
  mov r1.x, l(27)
  ushr r1.x, r2.x, r1.x
  mov r1.y, l(1)
  and r1.x, r1.y, r1.x
  ine r1.x, l(0, 0, 0, 0), r1.x
  uge r1.y, r1.w, cb0[2].z
  and r1.x, r1.y, r1.x
  ult r1.y, r1.w, cb0[2].w
  and r1.x, r1.y, r1.x
  if_nz r1.x
    mov r3.xyz, r2.yzwy
  endif 
  mov r1.x, l(31)
  ushr r1.x, r2.x, r1.x
  ine r1.x, l(0, 0, 0, 0), r1.x
  uge r1.y, r1.w, cb0[2].x
  and r1.x, r1.y, r1.x
  ult r1.y, r1.w, cb0[2].y
  and r1.x, r1.y, r1.x
  if_nz r1.x
    mov r3.xyz, r2.yzwy
  endif 
else 
  nop 
  mov r1.x, cb0[1].w
  and r1.y, r1.x, l(255)
  mov r1.z, l(8)
  ushr r1.z, r1.x, r1.z
  and r1.z, r1.z, l(255)
  mov r1.w, l(16)
  ushr r1.x, r1.x, r1.w
  and r1.x, r1.x, l(255)
  itof r2.x, r1.y
  itof r2.y, r1.z
  itof r2.z, r1.x
  div r3.xyz, r2.xyzx, l(255.000000, 255.000000, 255.000000, 0.000000)
  mov r3.xyz, r3.xyzx
endif 
itof r3.w, l(1)
mov r3.xyz, r3.xyzx
store_uav_typed u0.xyzw, r0.xzwy, r3.xyzw
ret 
// Approximately 0 instruction slots used
#endif

const BYTE ReftermCSShaderBytes[] =
{
     68,  88,  66,  67,  21,  85, 
     78,  40,  75,  75, 229, 253, 
      1,  47,  91, 109,  79, 246, 
    166,  73,   1,   0,   0,   0, 
     40,  14,   0,   0,   4,   0, 
      0,   0,  48,   0,   0,   0, 
     64,   0,   0,   0,  80,   0, 
      0,   0, 244,  13,   0,   0, 
     73,  83,  71,  78,   8,   0, 
      0,   0,   0,   0,   0,   0, 
      8,   0,   0,   0,  79,  83, 
     71,  78,   8,   0,   0,   0, 
      0,   0,   0,   0,   8,   0, 
      0,   0,  83,  72,  69,  88, 
    156,  13,   0,   0,  80,   0, 
      5,   0, 103,   3,   0,   0, 
    106, 136,   0,   1,  89,   0, 
      0,   4,  70, 142,  32,   0, 
      0,   0,   0,   0,   3,   0, 
      0,   0, 162,   0,   0,   4, 
      0, 112,  16,   0,   0,   0, 
      0,   0,  12,   0,   0,   0, 
     88,  24,   0,   4,   0, 112, 
     16,   0,   1,   0,   0,   0, 
     85,  85,   0,   0, 156,  24, 
      0,   4,   0, 224,  17,   0, 
      0,   0,   0,   0,  85,  85, 
      0,   0,  95,   0,   0,   2, 
     50,   0,   2,   0, 104,   0, 
      0,   2,   6,   0,   0,   0, 
    155,   0,   0,   4,   8,   0, 
      0,   0,   8,   0,   0,   0, 
      1,   0,   0,   0,  54,   0, 
      0,   4, 242,   0,  16,   0, 
      0,   0,   0,   0,  70,   5, 
      2,   0,  58,   0,   0,   1, 
     54,   0,   0,   5,  50,   0, 
     16,   0,   0,   0,   0,   0, 
     70,   0,  16,   0,   0,   0, 
      0,   0,  40,   0,   0,   6, 
     50,   0,  16,   0,   1,   0, 
      0,   0,  70, 128,  32,   0, 
      0,   0,   0,   0,   1,   0, 
      0,   0,  30,   0,   0,   7, 
     50,   0,  16,   0,   1,   0, 
      0,   0,  70,   0,  16,   0, 
      0,   0,   0,   0,  70,   0, 
     16,   0,   1,   0,   0,   0, 
     78,   0,   0,   9,  50,   0, 
     16,   0,   1,   0,   0,   0, 
      0, 208,   0,   0,  70,   0, 
     16,   0,   1,   0,   0,   0, 
     70, 128,  32,   0,   0,   0, 
      0,   0,   0,   0,   0,   0, 
     40,   0,   0,   6, 194,   0, 
     16,   0,   1,   0,   0,   0, 
      6, 132,  32,   0,   0,   0, 
      0,   0,   1,   0,   0,   0, 
     30,   0,   0,   7, 194,   0, 
     16,   0,   1,   0,   0,   0, 
      6,   4,  16,   0,   0,   0, 
      0,   0, 166,  14,  16,   0, 
      1,   0,   0,   0,  78,   0, 
      0,   9,   0, 208,   0,   0, 
    194,   0,  16,   0,   1,   0, 
      0,   0, 166,  14,  16,   0, 
      1,   0,   0,   0,   6, 132, 
     32,   0,   0,   0,   0,   0, 
      0,   0,   0,   0,  80,   0, 
      0,   8,  18,   0,  16,   0, 
      2,   0,   0,   0,  10,   0, 
     16,   0,   0,   0,   0,   0, 
     10, 128,  32,   0,   0,   0, 
      0,   0,   1,   0,   0,   0, 
     80,   0,   0,   8,  34,   0, 
     16,   0,   2,   0,   0,   0, 
     26,   0,  16,   0,   0,   0, 
      0,   0,  26, 128,  32,   0, 
      0,   0,   0,   0,   1,   0, 
      0,   0,   1,   0,   0,   7, 
     18,   0,  16,   0,   2,   0, 
      0,   0,  26,   0,  16,   0, 
      2,   0,   0,   0,  10,   0, 
     16,   0,   2,   0,   0,   0, 
     79,   0,   0,   8,  34,   0, 
     16,   0,   2,   0,   0,   0, 
     10,   0,  16,   0,   1,   0, 
      0,   0,  42, 128,  32,   0, 
      0,   0,   0,   0,   0,   0, 
      0,   0,   1,   0,   0,   7, 
     18,   0,  16,   0,   2,   0, 
      0,   0,  26,   0,  16,   0, 
      2,   0,   0,   0,  10,   0, 
     16,   0,   2,   0,   0,   0, 
     79,   0,   0,   8,  34,   0, 
     16,   0,   2,   0,   0,   0, 
     26,   0,  16,   0,   1,   0, 
      0,   0,  58, 128,  32,   0, 
      0,   0,   0,   0,   0,   0, 
      0,   0,   1,   0,   0,   7, 
     18,   0,  16,   0,   2,   0, 
      0,   0,  26,   0,  16,   0, 
      2,   0,   0,   0,  10,   0, 
     16,   0,   2,   0,   0,   0, 
     31,   0,   4,   3,  10,   0, 
     16,   0,   2,   0,   0,   0, 
     54,   0,   0,   6,  18,   0, 
     16,   0,   2,   0,   0,   0, 
     42, 128,  32,   0,   0,   0, 
      0,   0,   0,   0,   0,   0, 
     38,   0,   0,   8,   0, 208, 
      0,   0,  34,   0,  16,   0, 
      1,   0,   0,   0,  26,   0, 
     16,   0,   1,   0,   0,   0, 
     10,   0,  16,   0,   2,   0, 
      0,   0,  30,   0,   0,   7, 
     18,   0,  16,   0,   1,   0, 
      0,   0,  10,   0,  16,   0, 
      1,   0,   0,   0,  26,   0, 
     16,   0,   1,   0,   0,   0, 
    167,   0,   0, 139,   2,  99, 
      0, 128, 131, 153,  25,   0, 
     34,   0,  16,   0,   1,   0, 
      0,   0,  10,   0,  16,   0, 
      1,   0,   0,   0,   1,  64, 
      0,   0,   0,   0,   0,   0, 
      6, 112,  16,   0,   0,   0, 
      0,   0, 167,   0,   0, 139, 
      2,  99,   0, 128, 131, 153, 
     25,   0,  18,   0,  16,   0, 
      2,   0,   0,   0,  10,   0, 
     16,   0,   1,   0,   0,   0, 
      1,  64,   0,   0,   4,   0, 
      0,   0,   6, 112,  16,   0, 
      0,   0,   0,   0, 167,   0, 
      0, 139,   2,  99,   0, 128, 
    131, 153,  25,   0,  18,   0, 
     16,   0,   1,   0,   0,   0, 
     10,   0,  16,   0,   1,   0, 
      0,   0,   1,  64,   0,   0, 
      8,   0,   0,   0,   6, 112, 
     16,   0,   0,   0,   0,   0, 
     58,   0,   0,   1,  54,   0, 
      0,   5,  34,   0,  16,   0, 
      1,   0,   0,   0,  26,   0, 
     16,   0,   1,   0,   0,   0, 
      1,   0,   0,   7,  18,   0, 
     16,   0,   3,   0,   0,   0, 
     26,   0,  16,   0,   1,   0, 
      0,   0,   1,  64,   0,   0, 
    255, 255,   0,   0,  54,   0, 
      0,   5,  34,   0,  16,   0, 
      2,   0,   0,   0,   1,  64, 
      0,   0,  16,   0,   0,   0, 
     85,   0,   0,   7,  34,   0, 
     16,   0,   3,   0,   0,   0, 
     26,   0,  16,   0,   1,   0, 
      0,   0,  26,   0,  16,   0, 
      2,   0,   0,   0,  54,   0, 
      0,   5,  18,   0,  16,   0, 
      3,   0,   0,   0,  10,   0, 
     16,   0,   3,   0,   0,   0, 
     54,   0,   0,   5,  34,   0, 
     16,   0,   3,   0,   0,   0, 
     26,   0,  16,   0,   3,   0, 
      0,   0,  38,   0,   0,   9, 
      0, 208,   0,   0,  98,   0, 
     16,   0,   2,   0,   0,   0, 
      6,   1,  16,   0,   3,   0, 
      0,   0,   6, 129,  32,   0, 
      0,   0,   0,   0,   0,   0, 
      0,   0,  30,   0,   0,   7, 
     50,   0,  16,   0,   3,   0, 
      0,   0, 230,  10,  16,   0, 
      1,   0,   0,   0, 150,   5, 
     16,   0,   2,   0,   0,   0, 
     54,   0,   0,   8, 194,   0, 
     16,   0,   3,   0,   0,   0, 
      2,  64,   0,   0,   0,   0, 
      0,   0,   0,   0,   0,   0, 
      0,   0,   0,   0,   0,   0, 
      0,   0,  45,   0,   0, 137, 
    194,   0,   0, 128,  67,  85, 
     21,   0, 242,   0,  16,   0, 
      3,   0,   0,   0,  70,  14, 
     16,   0,   3,   0,   0,   0, 
     70, 126,  16,   0,   1,   0, 
      0,   0,  58,   0,   0,   1, 
     54,   0,   0,   5,  18,   0, 
     16,   0,   1,   0,   0,   0, 
     10,   0,  16,   0,   1,   0, 
      0,   0,   1,   0,   0,   7, 
     34,   0,  16,   0,   1,   0, 
      0,   0,  10,   0,  16,   0, 
      1,   0,   0,   0,   1,  64, 
      0,   0, 255,   0,   0,   0, 
     54,   0,   0,   5,  66,   0, 
     16,   0,   1,   0,   0,   0, 
      1,  64,   0,   0,   8,   0, 
      0,   0,  85,   0,   0,   7, 
     66,   0,  16,   0,   1,   0, 
      0,   0,  10,   0,  16,   0, 
      1,   0,   0,   0,  42,   0, 
     16,   0,   1,   0,   0,   0, 
      1,   0,   0,   7,  66,   0, 
     16,   0,   1,   0,   0,   0, 
     42,   0,  16,   0,   1,   0, 
      0,   0,   1,  64,   0,   0, 
    255,   0,   0,   0,  54,   0, 
      0,   5,  34,   0,  16,   0, 
      2,   0,   0,   0,   1,  64, 
      0,   0,  16,   0,   0,   0, 
     85,   0,   0,   7,  18,   0, 
     16,   0,   1,   0,   0,   0, 
     10,   0,  16,   0,   1,   0, 
      0,   0,  26,   0,  16,   0, 
      2,   0,   0,   0,   1,   0, 
      0,   7,  18,   0,  16,   0, 
      1,   0,   0,   0,  10,   0, 
     16,   0,   1,   0,   0,   0, 
      1,  64,   0,   0, 255,   0, 
      0,   0,  43,   0,   0,   5, 
     18,   0,  16,   0,   4,   0, 
      0,   0,  26,   0,  16,   0, 
      1,   0,   0,   0,  43,   0, 
      0,   5,  34,   0,  16,   0, 
      4,   0,   0,   0,  42,   0, 
     16,   0,   1,   0,   0,   0, 
     43,   0,   0,   5,  66,   0, 
     16,   0,   4,   0,   0,   0, 
     10,   0,  16,   0,   1,   0, 
      0,   0,  14,   0,   0,  10, 
    114,   0,  16,   0,   1,   0, 
      0,   0,  70,   2,  16,   0, 
      4,   0,   0,   0,   2,  64, 
      0,   0,   0,   0, 127,  67, 
      0,   0, 127,  67,   0,   0, 
    127,  67,   0,   0,   0,   0, 
     54,   0,   0,   5, 114,   0, 
     16,   0,   1,   0,   0,   0, 
     70,   2,  16,   0,   1,   0, 
      0,   0,  58,   0,   0,   1, 
     54,   0,   0,   5,  18,   0, 
     16,   0,   2,   0,   0,   0, 
     10,   0,  16,   0,   2,   0, 
      0,   0,   1,   0,   0,   7, 
     34,   0,  16,   0,   2,   0, 
      0,   0,  10,   0,  16,   0, 
      2,   0,   0,   0,   1,  64, 
      0,   0, 255,   0,   0,   0, 
     54,   0,   0,   5,  66,   0, 
     16,   0,   2,   0,   0,   0, 
      1,  64,   0,   0,   8,   0, 
      0,   0,  85,   0,   0,   7, 
     66,   0,  16,   0,   2,   0, 
      0,   0,  10,   0,  16,   0, 
      2,   0,   0,   0,  42,   0, 
     16,   0,   2,   0,   0,   0, 
      1,   0,   0,   7,  66,   0, 
     16,   0,   2,   0,   0,   0, 
     42,   0,  16,   0,   2,   0, 
      0,   0,   1,  64,   0,   0, 
    255,   0,   0,   0,  54,   0, 
      0,   5, 130,   0,  16,   0, 
      2,   0,   0,   0,   1,  64, 
      0,   0,  16,   0,   0,   0, 
     85,   0,   0,   7, 130,   0, 
     16,   0,   2,   0,   0,   0, 
     10,   0,  16,   0,   2,   0, 
      0,   0,  58,   0,  16,   0, 
      2,   0,   0,   0,   1,   0, 
      0,   7, 130,   0,  16,   0, 
      2,   0,   0,   0,  58,   0, 
     16,   0,   2,   0,   0,   0, 
      1,  64,   0,   0, 255,   0, 
      0,   0,  43,   0,   0,   5, 
     18,   0,  16,   0,   4,   0, 
      0,   0,  26,   0,  16,   0, 
      2,   0,   0,   0,  43,   0, 
      0,   5,  34,   0,  16,   0, 
      4,   0,   0,   0,  42,   0, 
     16,   0,   2,   0,   0,   0, 
     43,   0,   0,   5,  66,   0, 
     16,   0,   4,   0,   0,   0, 
     58,   0,  16,   0,   2,   0, 
      0,   0,  14,   0,   0,  10, 
    226,   0,  16,   0,   2,   0, 
      0,   0,   6,   9,  16,   0, 
      4,   0,   0,   0,   2,  64, 
      0,   0,   0,   0,   0,   0, 
      0,   0, 127,  67,   0,   0, 
    127,  67,   0,   0, 127,  67, 
     54,   0,   0,   5, 226,   0, 
     16,   0,   2,   0,   0,   0, 
     86,  14,  16,   0,   2,   0, 
      0,   0,  58,   0,   0,   1, 
     54,   0,   0,   6,  18,   0, 
     16,   0,   4,   0,   0,   0, 
     42, 128,  32,   0,   0,   0, 
      0,   0,   1,   0,   0,   0, 
      1,   0,   0,   7,  34,   0, 
     16,   0,   4,   0,   0,   0, 
     10,   0,  16,   0,   4,   0, 
      0,   0,   1,  64,   0,   0, 
    255,   0,   0,   0,  54,   0, 
      0,   5,  66,   0,  16,   0, 
      4,   0,   0,   0,   1,  64, 
      0,   0,   8,   0,   0,   0, 
     85,   0,   0,   7,  66,   0, 
     16,   0,   4,   0,   0,   0, 
     10,   0,  16,   0,   4,   0, 
      0,   0,  42,   0,  16,   0, 
      4,   0,   0,   0,   1,   0, 
      0,   7,  66,   0,  16,   0, 
      4,   0,   0,   0,  42,   0, 
     16,   0,   4,   0,   0,   0, 
      1,  64,   0,   0, 255,   0, 
      0,   0,  54,   0,   0,   5, 
    130,   0,  16,   0,   4,   0, 
      0,   0,   1,  64,   0,   0, 
     16,   0,   0,   0,  85,   0, 
      0,   7,  18,   0,  16,   0, 
      4,   0,   0,   0,  10,   0, 
     16,   0,   4,   0,   0,   0, 
     58,   0,  16,   0,   4,   0, 
      0,   0,   1,   0,   0,   7, 
     18,   0,  16,   0,   4,   0, 
      0,   0,  10,   0,  16,   0, 
      4,   0,   0,   0,   1,  64, 
      0,   0, 255,   0,   0,   0, 
     43,   0,   0,   5,  18,   0, 
     16,   0,   5,   0,   0,   0, 
     26,   0,  16,   0,   4,   0, 
      0,   0,  43,   0,   0,   5, 
     34,   0,  16,   0,   5,   0, 
      0,   0,  42,   0,  16,   0, 
      4,   0,   0,   0,  43,   0, 
      0,   5,  66,   0,  16,   0, 
      5,   0,   0,   0,  10,   0, 
     16,   0,   4,   0,   0,   0, 
     14,   0,   0,  10, 114,   0, 
     16,   0,   4,   0,   0,   0, 
     70,   2,  16,   0,   5,   0, 
      0,   0,   2,  64,   0,   0, 
      0,   0, 127,  67,   0,   0, 
    127,  67,   0,   0, 127,  67, 
      0,   0,   0,   0,  54,   0, 
      0,   5, 114,   0,  16,   0, 
      4,   0,   0,   0,  70,   2, 
     16,   0,   4,   0,   0,   0, 
     54,   0,   0,   5, 130,   0, 
     16,   0,   4,   0,   0,   0, 
      1,  64,   0,   0,  28,   0, 
      0,   0,  85,   0,   0,   7, 
    130,   0,  16,   0,   4,   0, 
      0,   0,  10,   0,  16,   0, 
      2,   0,   0,   0,  58,   0, 
     16,   0,   4,   0,   0,   0, 
     54,   0,   0,   5,  18,   0, 
     16,   0,   5,   0,   0,   0, 
      1,  64,   0,   0,   1,   0, 
      0,   0,   1,   0,   0,   7, 
    130,   0,  16,   0,   4,   0, 
      0,   0,  58,   0,  16,   0, 
      4,   0,   0,   0,  10,   0, 
     16,   0,   5,   0,   0,   0, 
     39,   0,   0,  10, 130,   0, 
     16,   0,   4,   0,   0,   0, 
      2,  64,   0,   0,   0,   0, 
      0,   0,   0,   0,   0,   0, 
      0,   0,   0,   0,   0,   0, 
      0,   0,  58,   0,  16,   0, 
      4,   0,   0,   0,  31,   0, 
      4,   3,  58,   0,  16,   0, 
      4,   0,   0,   0,  56,   0, 
      0,   7, 226,   0,  16,   0, 
      2,   0,   0,   0,  86,  14, 
     16,   0,   2,   0,   0,   0, 
      6,   9,  16,   0,   4,   0, 
      0,   0,  21,   0,   0,   1, 
     54,   0,   0,   5,  18,   0, 
     16,   0,   4,   0,   0,   0, 
      1,  64,   0,   0,  25,   0, 
      0,   0,  85,   0,   0,   7, 
     18,   0,  16,   0,   4,   0, 
      0,   0,  10,   0,  16,   0, 
      2,   0,   0,   0,  10,   0, 
     16,   0,   4,   0,   0,   0, 
     54,   0,   0,   5,  34,   0, 
     16,   0,   4,   0,   0,   0, 
      1,  64,   0,   0,   1,   0, 
      0,   0,   1,   0,   0,   7, 
     18,   0,  16,   0,   4,   0, 
      0,   0,  26,   0,  16,   0, 
      4,   0,   0,   0,  10,   0, 
     16,   0,   4,   0,   0,   0, 
     39,   0,   0,  10,  18,   0, 
     16,   0,   4,   0,   0,   0, 
      2,  64,   0,   0,   0,   0, 
      0,   0,   0,   0,   0,   0, 
      0,   0,   0,   0,   0,   0, 
      0,   0,  10,   0,  16,   0, 
      4,   0,   0,   0,  31,   0, 
      4,   3,  10,   0,  16,   0, 
      4,   0,   0,   0,  56,   0, 
      0,  10, 226,   0,  16,   0, 
      2,   0,   0,   0,  86,  14, 
     16,   0,   2,   0,   0,   0, 
      2,  64,   0,   0,   0,   0, 
      0,   0,   0,   0,   0,  63, 
      0,   0,   0,  63,   0,   0, 
      0,  63,  21,   0,   0,   1, 
     43,   0,   0,   5,  18,   0, 
     16,   0,   4,   0,   0,   0, 
      1,  64,   0,   0,   1,   0, 
      0,   0,  54,   0,   0,   6, 
    130,   0,  16,   0,   3,   0, 
      0,   0,  58,   0,  16, 128, 
     65,   0,   0,   0,   3,   0, 
      0,   0,   0,   0,   0,   7, 
    130,   0,  16,   0,   3,   0, 
      0,   0,  58,   0,  16,   0, 
      3,   0,   0,   0,  10,   0, 
     16,   0,   4,   0,   0,   0, 
     56,   0,   0,   7, 114,   0, 
     16,   0,   1,   0,   0,   0, 
     70,   2,  16,   0,   1,   0, 
      0,   0, 246,  15,  16,   0, 
      3,   0,   0,   0,  56,   0, 
      0,   7, 114,   0,  16,   0, 
      3,   0,   0,   0, 150,   7, 
     16,   0,   2,   0,   0,   0, 
     70,   2,  16,   0,   3,   0, 
      0,   0,   0,   0,   0,   7, 
    114,   0,  16,   0,   3,   0, 
      0,   0,  70,   2,  16,   0, 
      1,   0,   0,   0,  70,   2, 
     16,   0,   3,   0,   0,   0, 
     54,   0,   0,   5,  18,   0, 
     16,   0,   1,   0,   0,   0, 
      1,  64,   0,   0,  27,   0, 
      0,   0,  85,   0,   0,   7, 
     18,   0,  16,   0,   1,   0, 
      0,   0,  10,   0,  16,   0, 
      2,   0,   0,   0,  10,   0, 
     16,   0,   1,   0,   0,   0, 
     54,   0,   0,   5,  34,   0, 
     16,   0,   1,   0,   0,   0, 
      1,  64,   0,   0,   1,   0, 
      0,   0,   1,   0,   0,   7, 
     18,   0,  16,   0,   1,   0, 
      0,   0,  26,   0,  16,   0, 
      1,   0,   0,   0,  10,   0, 
     16,   0,   1,   0,   0,   0, 
     39,   0,   0,  10,  18,   0, 
     16,   0,   1,   0,   0,   0, 
      2,  64,   0,   0,   0,   0, 
      0,   0,   0,   0,   0,   0, 
      0,   0,   0,   0,   0,   0, 
      0,   0,  10,   0,  16,   0, 
      1,   0,   0,   0,  80,   0, 
      0,   8,  34,   0,  16,   0, 
      1,   0,   0,   0,  58,   0, 
     16,   0,   1,   0,   0,   0, 
     42, 128,  32,   0,   0,   0, 
      0,   0,   2,   0,   0,   0, 
      1,   0,   0,   7,  18,   0, 
     16,   0,   1,   0,   0,   0, 
     26,   0,  16,   0,   1,   0, 
      0,   0,  10,   0,  16,   0, 
      1,   0,   0,   0,  79,   0, 
      0,   8,  34,   0,  16,   0, 
      1,   0,   0,   0,  58,   0, 
     16,   0,   1,   0,   0,   0, 
     58, 128,  32,   0,   0,   0, 
      0,   0,   2,   0,   0,   0, 
      1,   0,   0,   7,  18,   0, 
     16,   0,   1,   0,   0,   0, 
     26,   0,  16,   0,   1,   0, 
      0,   0,  10,   0,  16,   0, 
      1,   0,   0,   0,  31,   0, 
      4,   3,  10,   0,  16,   0, 
      1,   0,   0,   0,  54,   0, 
      0,   5, 114,   0,  16,   0, 
      3,   0,   0,   0, 150,   7, 
     16,   0,   2,   0,   0,   0, 
     21,   0,   0,   1,  54,   0, 
      0,   5,  18,   0,  16,   0, 
      1,   0,   0,   0,   1,  64, 
      0,   0,  31,   0,   0,   0, 
     85,   0,   0,   7,  18,   0, 
     16,   0,   1,   0,   0,   0, 
     10,   0,  16,   0,   2,   0, 
      0,   0,  10,   0,  16,   0, 
      1,   0,   0,   0,  39,   0, 
      0,  10,  18,   0,  16,   0, 
      1,   0,   0,   0,   2,  64, 
      0,   0,   0,   0,   0,   0, 
      0,   0,   0,   0,   0,   0, 
      0,   0,   0,   0,   0,   0, 
     10,   0,  16,   0,   1,   0, 
      0,   0,  80,   0,   0,   8, 
     34,   0,  16,   0,   1,   0, 
      0,   0,  58,   0,  16,   0, 
      1,   0,   0,   0,  10, 128, 
     32,   0,   0,   0,   0,   0, 
      2,   0,   0,   0,   1,   0, 
      0,   7,  18,   0,  16,   0, 
      1,   0,   0,   0,  26,   0, 
     16,   0,   1,   0,   0,   0, 
     10,   0,  16,   0,   1,   0, 
      0,   0,  79,   0,   0,   8, 
     34,   0,  16,   0,   1,   0, 
      0,   0,  58,   0,  16,   0, 
      1,   0,   0,   0,  26, 128, 
     32,   0,   0,   0,   0,   0, 
      2,   0,   0,   0,   1,   0, 
      0,   7,  18,   0,  16,   0, 
      1,   0,   0,   0,  26,   0, 
     16,   0,   1,   0,   0,   0, 
     10,   0,  16,   0,   1,   0, 
      0,   0,  31,   0,   4,   3, 
     10,   0,  16,   0,   1,   0, 
      0,   0,  54,   0,   0,   5, 
    114,   0,  16,   0,   3,   0, 
      0,   0, 150,   7,  16,   0, 
      2,   0,   0,   0,  21,   0, 
      0,   1,  18,   0,   0,   1, 
     58,   0,   0,   1,  54,   0, 
      0,   6,  18,   0,  16,   0, 
      1,   0,   0,   0,  58, 128, 
     32,   0,   0,   0,   0,   0, 
      1,   0,   0,   0,   1,   0, 
      0,   7,  34,   0,  16,   0, 
      1,   0,   0,   0,  10,   0, 
     16,   0,   1,   0,   0,   0, 
      1,  64,   0,   0, 255,   0, 
      0,   0,  54,   0,   0,   5, 
     66,   0,  16,   0,   1,   0, 
      0,   0,   1,  64,   0,   0, 
      8,   0,   0,   0,  85,   0, 
      0,   7,  66,   0,  16,   0, 
      1,   0,   0,   0,  10,   0, 
     16,   0,   1,   0,   0,   0, 
     42,   0,  16,   0,   1,   0, 
      0,   0,   1,   0,   0,   7, 
     66,   0,  16,   0,   1,   0, 
      0,   0,  42,   0,  16,   0, 
      1,   0,   0,   0,   1,  64, 
      0,   0, 255,   0,   0,   0, 
     54,   0,   0,   5, 130,   0, 
     16,   0,   1,   0,   0,   0, 
      1,  64,   0,   0,  16,   0, 
      0,   0,  85,   0,   0,   7, 
     18,   0,  16,   0,   1,   0, 
      0,   0,  10,   0,  16,   0, 
      1,   0,   0,   0,  58,   0, 
     16,   0,   1,   0,   0,   0, 
      1,   0,   0,   7,  18,   0, 
     16,   0,   1,   0,   0,   0, 
     10,   0,  16,   0,   1,   0, 
      0,   0,   1,  64,   0,   0, 
    255,   0,   0,   0,  43,   0, 
      0,   5,  18,   0,  16,   0, 
      2,   0,   0,   0,  26,   0, 
     16,   0,   1,   0,   0,   0, 
     43,   0,   0,   5,  34,   0, 
     16,   0,   2,   0,   0,   0, 
     42,   0,  16,   0,   1,   0, 
      0,   0,  43,   0,   0,   5, 
     66,   0,  16,   0,   2,   0, 
      0,   0,  10,   0,  16,   0, 
      1,   0,   0,   0,  14,   0, 
      0,  10, 114,   0,  16,   0, 
      3,   0,   0,   0,  70,   2, 
     16,   0,   2,   0,   0,   0, 
      2,  64,   0,   0,   0,   0, 
    127,  67,   0,   0, 127,  67, 
      0,   0, 127,  67,   0,   0, 
      0,   0,  54,   0,   0,   5, 
    114,   0,  16,   0,   3,   0, 
      0,   0,  70,   2,  16,   0, 
      3,   0,   0,   0,  21,   0, 
      0,   1,  43,   0,   0,   5, 
    130,   0,  16,   0,   3,   0, 
      0,   0,   1,  64,   0,   0, 
      1,   0,   0,   0,  54,   0, 
      0,   5, 114,   0,  16,   0, 
      3,   0,   0,   0,  70,   2, 
     16,   0,   3,   0,   0,   0, 
    164,   0,   0,   7, 242, 224, 
     17,   0,   0,   0,   0,   0, 
    134,   7,  16,   0,   0,   0, 
      0,   0,  70,  14,  16,   0, 
      3,   0,   0,   0,  62,   0, 
      0,   1,  73,  76,  68,  78, 
     44,   0,   0,   0,   0,   0, 
     36,   0,  50,  99,  48,  49, 
     55,  50,  97,  97, 102, 102, 
     56,  98, 102,  52,  97,  98, 
     98, 101,  53,  57,  56,  50, 
     53,  97,  49,  98,  55,  50, 
     50,  99,  98, 102,  46, 112, 
    100,  98,   0,   0,   0,   0
};
