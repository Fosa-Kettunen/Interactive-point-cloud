# Interactive point cloud
This is a point cloud demo that Uses Unity's VFX Graph and GPU to render an interactive pointcloud.
Download windows build [here](https://github.com/Fosa-Kettunen/Interactive-point-cloud/releases/download/v1.0.0/Interactive-point-cloud-build-v1.0.0.zip)

![image](https://github.com/user-attachments/assets/a915c4c1-3f66-486a-90c0-594cd1d1410c)


# Features
This demo has three different types of effects:
1. Passive weave: Looping animation.
2. Interactive ball collider: Follows mouse.
3. On-Click weave: An effect triggered by mouse clicks.

# Changing the point cloud model
## Create pcache file
pcache files are essentially just ply files with some modifications.

1. Change the ".ply" ending to ".pcache".
2. Format the header to match the pcache header format below. Note that we only need element count, position and color. 
**What pcache file looks like:**
```
pcache
format ascii 1.0
elements 910310
property float position.x
property float position.z
property float position.y
property uchar color.r
property uchar color.g
property uchar color.b
end_header
```

**What common ply file looks like:**
```
ply
format binary_little_endian 1.0
comment VCGLIB generated
element vertex 910310
property float x
property float y
property float z
property float nx
property float ny
property float nz
property uchar red
property uchar green
property uchar blue
property uchar alpha
element face 0
property list uchar int vertex_indices
end_header
```

4. Move the pcache-file to "Assets" folder.
5. Take a note of element count. It will be used in later scripts.

## Edit values in scene
### Visual Effect
1. In game object "Visual Effect" or in folder Assets, click "Pointcloud" component to open VFX graph.
2. Change the asset to one you want.

![image](https://github.com/user-attachments/assets/1b980fbf-d46c-4a34-8ab8-17b58a73300d)

4. In "Spawner" and "Initialize" nodes, change the "Count" and "Capacity" to match the element count of the model.

![image](https://github.com/user-attachments/assets/fcb61276-ff53-4691-ac02-3d053410429a)

### scripts
1. In scene find "WeaveEvent" script. In data section change the position texture to your models position texture.
2. In the same component match "Read Elements" to the object’s element count.

# Credits
PLY model used in this project.
"A point cloud of roses" (https://skfb.ly/6zWS8) by alban is licensed under Creative Commons Attribution (http://creativecommons.org/licenses/by/4.0/).
