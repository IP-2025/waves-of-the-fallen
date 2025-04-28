import { Column, Entity, JoinColumn, OneToOne, PrimaryColumn } from 'typeorm';

@Entity()
export class Character {

  @PrimaryColumn({
    type: 'int',
    nullable: false,
    unique: true,
  })
  character_id!: number;

  @Column({
    type: 'varchar',
    nullable: false,
    unique: true,
  })
  name!: string;

  @Column({
    type: 'int',
    nullable: false,
  })
  speed!: number;

  @Column({
    type: 'int',
    nullable: false,
  })
  health!: number;

  @Column({
    type: 'int',
    nullable: false,
  })
  dexterity!: number;

  @Column({
    type: 'int',
    nullable: false,
  })
  intelligence!: number;
}
